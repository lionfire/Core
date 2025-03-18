// ENH maybe: for now just support the one approach of nameless documents.  Uncommenting this would allow for the document file to carry the matching name of the parent directory, which may make for more ease of use in some situations (such as having multiple files open in an editor.)
//#define DocumentWithMatchingKeyName
using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;
using LionFire.IO.Reactive.Filesystem;
using LionFire.Persistence.Filesystemlike;
using LionFire.Reactive;
using LionFire.Reactive.Persistence;
using LionFire.Structures;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using System.Threading;
using Path = System.IO.Path;

namespace LionFire.IO.Reactive;


public abstract class FsDirectoryReaderRx<TKey, TValue>
        : DirectoryReaderRx<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
{
    protected FsDirectoryReaderRx(DirectorySelector dir, IFileExtensionConvention extensionConvention) : base(dir, extensionConvention)
    {
    }

    protected override IDirectoryAsync Directory => FsDirectory.Instance;
}


// TODO REFACTOR: Move common logic to a base class, though I'm not sure how much common logic there is.
public abstract class DirectoryReaderRx<TKey, TValue>
    : ObservableReaderBase<TKey, TValue>
    , IObservableReader<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    protected abstract IDirectoryAsync Directory { get; }
    protected abstract string Extension { get; }

    /// <summary>
    /// Without leading dot
    /// </summary>
    protected virtual string SecondExtension => ExtensionConvention.FileExtensionForType(typeof(TValue));

    public static string ToKebabCase(string input)
    {
        // Replace spaces with hyphens and convert to lowercase
        return Regex.Replace(input, @"\s+", "-").ToLower();
    }

    #region Parameters

    public DirectorySelector Dir { get; }
    public IFileExtensionConvention ExtensionConvention { get; }

    #endregion

    #region Lifecycle

    public DirectoryReaderRx(DirectorySelector dir, IFileExtensionConvention extensionConvention)
    {
        Dir = dir;
        ExtensionConvention = extensionConvention;
        _ = LoadKeys();
        //_ = LoadValues();

        // REVIEW: how do deletions get handled?  When ReadFromFile returns null, it should be removed from the cache.

        //IObservable<IChangeSet<Change<KeyValuePair<TKey, TValue>>, TKey>> x 
        //    = keys.ToObservableChangeSet()
        //        .Transform(key => new KeyValuePair<TKey, TValue>(key, ReadFromFile(GetFilePath(key))!))
        //        .Filter(x => x.Value != null)
        //        .ToObservableChangeSet(kvp => kvp.Item.Current.Key)
        //        //.AsObservableCache()
        //    ;
        // Convert x to Items

        // Transform keys to KeyValuePair<TKey, TValue> and filter out null values
        //KeyedItems = keys.Connect()
        //    .Transform(key => new KeyValuePair<TKey, TValue>(key, ReadFromFile(GetFilePath(key))!))
        //    //.AddKey(kvp => kvp.Key)
        //    //.Flatten()
        //    //.Where(change => change.Current.Value != null)
        //    .AsObservableCache()
        //    ;

        var published = Values
            .Connect()
            .Publish();

        ObservableCache = published
            //.Transform(x => x.optional)
            .AsObservableCache();

        IDisposable? listenAllSubscription = null;

        // Track subscriber count
        var subscriberCount = published
            .SubscribeCount() // Custom extension method (defined below)
            .Subscribe(count =>
            {
                Console.WriteLine($"Subscriber count changed to: {count}");
                if (count == 1)
                {
                    listenAllSubscription = ListenAll();
                    //LoadValues();
                }
                else if (count == 0)
                {
                    listenAllSubscription?.Dispose();
                    //StopListeningForValues();
                }
            });

        // Connect the shared observable
        _disposables.Add(published.Connect());
        _disposables.Add(subscriberCount);
        _disposables.Add(ObservableCache);
    }
    private readonly CompositeDisposable _disposables = new();

    #endregion


    #region State

    public IObservableCache<TKey, TKey> Keys => keys.AsObservableCache();
    //IObservableList<TKey> IObservableReader<TKey, TValue>.Keys
    //{
    //    get
    //    {
    //        var observableList = new ObservableCollection<TKey>();

    //        keys.Connect().Transform(x => x)
    //         .Bind(observableList)
    //         .Subscribe();

    //        return new ObservableListAdapter<TKey>(observableList);
    //    }
    //}
    //private readonly ObservableCollection<TKey> keys = new();
    private readonly SourceCache<TKey, TKey> keys = new(k => k); // REVIEW - why isn't this a SourceList?


    // ENH: Implement this in one of various ways:
    // - identical to KeyedItems (ObservableCache), but remove items when listening stops.  KeyedItems is still useful for one-shot (manual) loads of data.
    public IObservableList<TKey> ListeningForValues => listeningForValues; // NOTIMPLEMENTED
    private SourceList<TKey> listeningForValues = new();

    public IObservableCache<Optional<TValue>, TKey> ObservableCache { get; }

    private int subscriberCount = 0;
    private readonly object subscriberLock = new object();

    #endregion

    public Func<string, string, Optional<TKey>> GetKeyForNameAndPath { get; set; } = (string name, string path) =>
    {
        name = name.TrimStart(Path.DirectorySeparatorChar).TrimStart(Path.AltDirectorySeparatorChar);

        if (name is TKey typedName)
        {
            return typedName;
        }

        return Optional.None<TKey>();
        //if (isStringKey)
        //{
        //    return (TKey)(object)name;
        //}
        //throw new NotImplementedException();
    };

    const bool AllowSingleFileAndMultiMode = true;

    public string RootDirName { get; set; } = "(root)";



    protected async ValueTask LoadKeys()
    {
        await Task.Run(async () =>
        {
            var namelessDocumentFile = SecondExtension + Extension;
            var namelessDocumentPath = Path.Combine(Dir.Path, namelessDocumentFile);

            if (await Directory.ExistsAsync(Dir.Path).ConfigureAwait(false))
            {
                var dirName = Path.GetDirectoryName(Dir.Path) ?? RootDirName;

                var o = new EnumerationOptions { RecurseSubdirectories = Dir.Recursive };
                var filesT = Directory.GetFilesAsync(Dir.Path, "*." + SecondExtension + Extension, o).ConfigureAwait(false);
                var files2 = await Directory.GetFilesAsync(Dir.Path, SecondExtension + Extension, o).ConfigureAwait(false);
                var files = (await filesT).Concat(files2); 

                bool isStringKey = typeof(TKey) == typeof(string);

                bool namelessDocumentFileExists = File.Exists(namelessDocumentPath); // Takes precedence
                //bool namelessDocumentFileExists = files.Contains(namelessDocumentFile); // Takes precedence
#if DocumentWithMatchingKeyName
                //bool dirNameDocumentFileExists = files.Contains(Path.Combine(Dir.Path, dirName + "." + SecondExtension + Extension));
#endif

                bool multiMode = AllowSingleFileAndMultiMode ||
                !namelessDocumentFileExists
#if DocumentWithMatchingKeyName
                && !dirNameDocumentFileExists
#endif
                ;

                if (namelessDocumentFileExists)
                {
                    HandleNameless(namelessDocumentFile, dirName);
                }
#if DocumentWithMatchingKeyName
                //else if (dirNameDocumentFileExists) { }
#endif
                if (multiMode)
                {
                    foreach (var file in files)
                    {
                        var fileDir = Path.GetDirectoryName(file) ?? "";
                        fileDir = fileDir.Replace('\\', '/');
                        var subDirsWithoutRoot = fileDir.Replace(Dir.Path, "").TrimStart(Path.DirectorySeparatorChar).TrimStart(Path.AltDirectorySeparatorChar);

                        var withoutExt = Path.GetFileNameWithoutExtension(file);
                        if (withoutExt == SecondExtension)
                        {
                            var key = GetKeyForNameAndPath(subDirsWithoutRoot, file);
                            if (key.HasValue) { keys.AddOrUpdate(key.Value); }
                        }
                        else
                        {
                            var nameFromFileName = Path.GetFileNameWithoutExtension(withoutExt);
                            var key = GetKeyForNameAndPath(Path.Combine(subDirsWithoutRoot, nameFromFileName), file);
                            if (key.HasValue) { keys.AddOrUpdate(key.Value); }
                        }
                    }
                }
            }

            void HandleNameless(string namelessDocumentFile, string dirName)
            {
                var key = GetKeyForNameAndPath(dirName, namelessDocumentFile);
                if (key.HasValue) { keys.AddOrUpdate(key.Value); }
            }
        });
    }

    public IDisposable ListenAll()
    {
        Interlocked.Increment(ref _activeListeners);
        var subscription = Disposable.Create(() =>
        {
            // Decrement counter and stop listening when no active listeners remain
            if (Interlocked.Decrement(ref _activeListeners) == 0)
            {
                StopListeningAll();
            }
        });
        StartListeningAll();
        return subscription;
    }
    private int _activeListeners = 0;

    void OnItem(TKey key, Optional<TValue> value)
    {
        Debug.WriteLine($"OnItem({key}, {value})");
        this.values.AddOrUpdate((key, value));
    }

    public IObservable<TValue?>? GetValueObservableIfExists(TKey key)
        => !Keys.Keys.Contains(key)
        ? null
        : valueObservables.GetOrAdd(key, k => CreateValueObservable(k)
            .Publish()
            .RefCount());

    public IObservable<TValue?> GetValueObservable(TKey key)
        => valueObservables.GetOrAdd(key, k => CreateValueObservable(k)
            .Publish()
            .RefCount());

    private ConcurrentDictionary<TKey, IObservable<TValue?>> valueObservables = new();
    // TODO: Remove keys from valueObservables when they go missing

    protected abstract IObservable<TValue?> CreateValueObservable(TKey key);

    SourceCache<(TKey key, IDisposable subscription), TKey> listenToAllListeners = new(i => i.key);
    IDisposable? listeningAllSubscription;
    private void StartListeningAll()
    {
        listeningAllSubscription = this.Keys.Connect().Subscribe(changeSet =>
        {
            listenToAllListeners.Edit(su =>
            {
                foreach (var change in changeSet)
                {
                    switch (change.Reason)
                    {
                        case ChangeReason.Add:
                            if (!su.Lookup(change.Key).HasValue)
                            {
                                var key = change.Key;
                                su.AddOrUpdate((change.Key, GetValueObservable(change.Key)
                                    .Subscribe(v => OnItem(key, v))));
                            }
                            break;
                        case ChangeReason.Remove:
                            var existing = su.Lookup(change.Key);
                            if (existing.HasValue)
                            {
                                existing.Value.subscription.Dispose();
                                su.RemoveKey(change.Key);
                            }
                            break;
                        case ChangeReason.Refresh:
                            throw new NotImplementedException();
                        //case ChangeReason.Moved:
                        //case ChangeReason.Update:
                        default:
                            break;
                    }

                }
            });
        });
    }

    private void StopListeningAll()
    {
        listeningAllSubscription?.Dispose();
        listenToAllListeners.Items.ToList().ForEach(i => i.subscription.Dispose());
        listenToAllListeners.Clear();
    }

    protected string GetFilePath(TKey key) => Path.Combine(Dir.Path ?? throw new ArgumentNullException(), $"{key}.{SecondExtension}{Extension}");

    protected abstract TValue? ReadFromFile(string filePath);

    //protected abstract void LoadValues();
    //protected abstract void StopListeningForValues();
}


// Custom extension method to track subscriber count
public static class ObservableExtensions
{
    public static IObservable<int> SubscribeCount<T>(this IObservable<T> source)
    {
        var count = 0;
        return Observable.Create<int>(observer =>
        {
            var subscription = source.Subscribe(
                _ => { }, // Ignore OnNext events
                observer.OnError,
                observer.OnCompleted);

            count++;
            observer.OnNext(count);

            return Disposable.Create(() =>
            {
                count--;
                observer.OnNext(count);
                subscription.Dispose();
            });
        });
    }
}