// ENH maybe: for now just support the one approach of nameless documents.  Uncommenting this would allow for the document file to carry the matching name of the parent directory, which may make for more ease of use in some situations (such as having multiple files open in an editor.)
//#define DocumentWithMatchingKeyName
using DynamicData;
using DynamicData.Kernel;
using LionFire.Ontology;
using LionFire.Persistence.Filesystemlike;
using LionFire.Reactive.Persistence;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Text.RegularExpressions;
using System.Threading;
using Path = System.IO.Path;
using LionFire.Reactive;
using LionFire.Dependencies;
using Microsoft.Extensions.Logging;

namespace LionFire.IO.Reactive;

// TODO REFACTOR: Move common logic to a base class or options class (DirectoryTypeOptions), though I'm not sure how much common logic there is.
// - Move more into DirectoryTypeOptions
public abstract class DirectoryReaderRx<TKey, TValue>
    : ObservableReaderBase<TKey, TValue>
    , IObservableReader<TKey, TValue>
    //, IHas<DirectoryTypeOptions>
    where TKey : notnull
    where TValue : notnull
{
    protected abstract IDirectoryAsync Directory { get; }
    protected abstract string Extension { get; }

    protected ILogger Logger { get; }

    #region Parameters

    public DirectorySelector Dir { get; }

    public DirectoryTypeOptions DirectoryTypeOptions { get; set; }

    #region Derived

    //DirectoryTypeOptions? IHas<DirectoryTypeOptions>.Object => DirectoryTypeOptions;
    public IFileExtensionConvention? ExtensionConvention => DirectoryTypeOptions.ExtensionConvention;

    /// <summary>
    /// Without leading dot
    /// </summary>
    protected virtual string? SecondExtension => ExtensionConvention?.FileExtensionForType(typeof(TValue));

    #endregion

    #endregion

    #region Lifecycle

    protected virtual async Task Initialize()
    {
        await LoadKeys();
    }

    public DirectoryReaderRx(DirectorySelector dir, DirectoryTypeOptions directoryTypeOptions, ILogger logger, bool deferInit = false)
    {
        Logger = logger;
        Dir = dir;
        DirectoryTypeOptions = directoryTypeOptions;

        if (!deferInit)
        {
            _ = Initialize();
        }
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
                Debug.WriteLine($"{this.GetType()} - Subscriber count changed to: {count}");
                if (count == 1)
                {
                    listenAllSubscription = ListenAllKeys();
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

    #region Keys watcher

    FileSystemWatcher? keysWatcher;

    private void EnableKeysWatcher()
    {
        Debug.WriteLine("Keys subscribe: " + Dir.Path);

        if (!System.IO.Directory.Exists(Dir.Path))
        {
            System.IO.Directory.CreateDirectory(Dir.Path);
        }

        keysWatcher = new FileSystemWatcher(Dir.Path)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            //Filter = $"*.{SecondExtension}{Extension}",
            IncludeSubdirectories = Dir.Recursive,
        };

        keysWatcher.Filters.Add($"*.{SecondExtension}{Extension}");
        keysWatcher.Filters.Add($"{SecondExtension}{Extension}");

        keysWatcher.Created += OnFileCreated;
        keysWatcher.Deleted += OnFileDeleted;
        keysWatcher.Renamed += OnFileRenamed;
        keysWatcher.Changed += OnFileChanged;
        keysWatcher.EnableRaisingEvents = true;
    }

    private void DisableKeysWatcher()
    {
        Debug.WriteLine("Keys unsubscribe: " + Dir.Path);

        if (keysWatcher != null)
        {
            keysWatcher.EnableRaisingEvents = false;
            keysWatcher.Created -= OnFileCreated;
            keysWatcher.Deleted -= OnFileDeleted;
            keysWatcher.Renamed -= OnFileRenamed;
            keysWatcher.Changed -= OnFileChanged;
            keysWatcher.Dispose();
            keysWatcher = null;
        }
    }


    /// <summary>
    /// Check if a file path is within the allowed recursion depth
    /// </summary>
    private bool IsWithinRecursionDepth(string fullPath)
    {
        if (!Dir.Recursive || Dir.RecursionDepth >= int.MaxValue)
        {
            return true; // No depth limit
        }

        return GetRelativeDepth(fullPath, Dir.Path) <= Dir.RecursionDepth;
    }

    // REVIEW - REFACTOR into GetKeyForNameAndPath? How is this different?  No subdir support?
    private Optional<TKey> KeysWatcher_GetKeyFromFileName(string fileName, string fullPath)
    {
        if (!IsWithinRecursionDepth(fullPath))
        {
            return Optional.None<TKey>();
        }

        // Extract key using the same logic as LoadKeys() to support subdirectories
        var fileDir = Path.GetDirectoryName(fullPath) ?? "";
        fileDir = fileDir.Replace('\\', '/');
        var subDirsWithoutRoot = fileDir.Replace(Dir.Path, "").TrimStart(Path.DirectorySeparatorChar).TrimStart(Path.AltDirectorySeparatorChar);

        var withoutExt = Path.GetFileNameWithoutExtension(fileName);
        if (withoutExt == SecondExtension)
        {
            // File is in subdirectory like "~New~XXX/portfolio.hjson" - use subdirectory name as key
            return GetKeyForNameAndPath(subDirsWithoutRoot, fullPath);
        }
        else
        {
            // File is flat like "P1.portfolio.hjson" - use filename as key
            var nameFromFileName = Path.GetFileNameWithoutExtension(withoutExt);
            var key = GetKeyForNameAndPath(Path.Combine(subDirsWithoutRoot, nameFromFileName), fullPath);
            return key;
        }
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        var key = KeysWatcher_GetKeyFromFileName(e.Name!, e.FullPath);
        if (key.HasValue)
        {
            keys.AddOrUpdate(key.Value);
        }
    }

    private void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        var key = KeysWatcher_GetKeyFromFileName(e.Name!, e.FullPath);
        if (key.HasValue)
        {
            keys.RemoveKey(key.Value);
        }
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        var oldKey = KeysWatcher_GetKeyFromFileName(e.OldName!, e.OldFullPath);
        var newKey = KeysWatcher_GetKeyFromFileName(e.Name!, e.FullPath);

        if (oldKey.HasValue)
        {
            keys.RemoveKey(oldKey.Value);
        }

        if (newKey.HasValue)
        {
            keys.AddOrUpdate(newKey.Value);
        }
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        var key = KeysWatcher_GetKeyFromFileName(e.Name!, e.FullPath);
        if (key.HasValue)
        {
            keys.AddOrUpdate(key.Value);
        }
    }

    #endregion


    #region State

    public IObservableCache<TKey, TKey> Keys => keysWithSubscribeEvents ??=
        keys
            .ConnectOnDemand(v => v)
            .PublishRefCountWithEvents(() => EnableKeysWatcher(), () => DisableKeysWatcher())
            .AsObservableCache();
    private IObservableCache<TKey, TKey>? keysWithSubscribeEvents;

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
    //public IObservableList<TKey> ListeningForValues => listeningForValues; // NOTIMPLEMENTED
    //private SourceList<TKey> listeningForValues = new();

    public override IObservableCache<Optional<TValue>, TKey> ObservableCache { get; }

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

    /// <summary>
    /// Calculate the depth of a file path relative to a base directory.
    /// Returns 0 for files in the base directory, 1 for files in immediate subdirectories, etc.
    /// </summary>
    private int GetRelativeDepth(string filePath, string basePath)
    {
        var fileDir = Path.GetDirectoryName(filePath) ?? "";
        var relativePath = Path.GetRelativePath(basePath, fileDir);

        if (relativePath == ".")
        {
            return 0; // File is in the base directory
        }

        // Count directory separators to determine depth
        return relativePath.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    const bool AllowSingleFileAndMultiMode = true;

    public string RootDirName { get; set; } = "(root)";



    protected async ValueTask LoadKeys()
    {
        Logger?.LogTrace($"[LoadKeys] Starting for {Dir.Path}, Recursive={Dir.Recursive}, RecursionDepth={Dir.RecursionDepth}");

        await Task.Run(async () =>
        {
            var namelessDocumentFile = SecondExtension + Extension;
            var namelessDocumentPath = Path.Combine(Dir.Path, namelessDocumentFile);

            if (await Directory.ExistsAsync(Dir.Path).ConfigureAwait(false))
            {
                Logger?.LogTrace($"[LoadKeys] Directory exists: {Dir.Path}");
                var dirName = Path.GetDirectoryName(Dir.Path) ?? RootDirName;

                var o = new EnumerationOptions { RecurseSubdirectories = Dir.Recursive };
                var filesT = Directory.GetFilesAsync(Dir.Path, "*." + SecondExtension + Extension, o).ConfigureAwait(false);
                var files2 = await Directory.GetFilesAsync(Dir.Path, SecondExtension + Extension, o).ConfigureAwait(false);
                var files = (await filesT).Concat(files2);

                Logger?.LogTrace($"[LoadKeys] Found {files.Count()} files before filtering");

                // Filter by recursion depth if specified
                if (Dir.Recursive && Dir.RecursionDepth < int.MaxValue)
                {
                    var allFiles = files.ToArray();
                    Logger?.LogTrace($"[RecursionDepth Filter] Found {allFiles.Length} files total before depth filtering (depth limit: {Dir.RecursionDepth})");
                    files = allFiles.Where(f =>
                    {
                        var depth = GetRelativeDepth(f, Dir.Path);
                        var included = depth <= Dir.RecursionDepth;
                        if (!included)
                        {
                            Logger?.LogTrace($"[RecursionDepth Filter] Excluding file at depth {depth}: {f}");
                        }
                        return included;
                    }).ToArray();
                    Logger?.LogTrace($"[RecursionDepth Filter] {files.Count()} files remaining after depth filtering");
                }

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
                    Logger?.LogTrace($"[LoadKeys] Processing {files.Count()} files in multiMode");
                    foreach (var file in files)
                    {
                        var fileDir = Path.GetDirectoryName(file) ?? "";
                        fileDir = fileDir.Replace('\\', '/');
                        var subDirsWithoutRoot = fileDir.Replace(Dir.Path, "").TrimStart(Path.DirectorySeparatorChar).TrimStart(Path.AltDirectorySeparatorChar);

                        var withoutExt = Path.GetFileNameWithoutExtension(file);
                        if (withoutExt == SecondExtension)
                        {
                            var key = GetKeyForNameAndPath(subDirsWithoutRoot, file);
                            if (key.HasValue)
                            {
                                Logger?.LogTrace($"[LoadKeys] Adding key from subdir: {key.Value}");
                                keys.AddOrUpdate(key.Value);
                            }
                        }
                        else
                        {
                            var nameFromFileName = Path.GetFileNameWithoutExtension(withoutExt);
                            var key = GetKeyForNameAndPath(Path.Combine(subDirsWithoutRoot, nameFromFileName), file);
                            if (key.HasValue)
                            {
                                Logger?.LogTrace($"[LoadKeys] Adding key from filename: {key.Value}");
                                keys.AddOrUpdate(key.Value);
                            }
                        }
                    }
                    Logger?.LogTrace($"[LoadKeys] Completed. Total keys in cache: {keys.Count}");
                }
            }

            void HandleNameless(string namelessDocumentFile, string dirName)
            {
                var key = GetKeyForNameAndPath(dirName, namelessDocumentFile);
                if (key.HasValue) { keys.AddOrUpdate(key.Value); }
            }
        });
    }

    public async ValueTask<IDisposable> ListenAllValues()
    {
        var newActiveListeners = Interlocked.Increment(ref _activeValueListeners);

        var subscription = Disposable.Create(() =>
        {
            // Decrement counter and stop listening when no active listeners remain
            if (Interlocked.Decrement(ref _activeValueListeners) == 0)
            {
                StopListeningToAllValues();
            }
        });
        if (newActiveListeners == 1)
        {
            await StartListeningToAllValues();
        }
        return subscription;
    }
    private volatile int _activeValueListeners = 0;

    public IDisposable ListenAllKeys()
    {
        var newActiveListeners = Interlocked.Increment(ref _activeKeyListeners);

        var subscription = Disposable.Create(() =>
        {
            // Decrement counter and stop listening when no active listeners remain
            if (Interlocked.Decrement(ref _activeKeyListeners) == 0)
            {
                StopListeningToAllKeys();
            }
        });
        if (newActiveListeners == 1)
        {
            StartListeningToAllKeys();
        }
        return subscription;
    }
    private volatile int _activeKeyListeners = 0;

    void OnValue(TKey key, Optional<TValue> value)
    {
        if (IsPendingWrite(key))
        {
            Logger?.LogTrace($"[OnValue] Self-generated change detected for '{key}': {value}");
        }
        else
        {
            Logger?.LogTrace($"[OnValue] Initial load or external change detected for '{key}': {value}");
            this.values.AddOrUpdate((key, value));
        }
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
    IDisposable? listeningAll_KeysSubscription;

    private async ValueTask StartListeningToAllValues()
    {
        var toLoad = values.KeyValues.Where(kv => !kv.Value.optional.HasValue);

        ValuesListeningToKeysSubscription = ListenAllKeys();

        //if (toLoad.Any())
        //{
        //    await Task.WhenAll(toLoad.Select(x => TryGetValue(x.Key).AsTask()));
        //}
    }

    //public ValueTask<Optional<TValue>> TryGetValue(TKey key)
    //{
    //    var result = values.Lookup(key);
    //    Debug.WriteLine($"TryGetValue {key}: already has value: {result.HasValue}");

    //    if (result.HasValue)
    //    {
    //        if (result.Value.optional.HasValue)
    //        {
    //            return ValueTask.FromResult(result.Value.optional);
    //        }
    //    }

    //    throw new NotImplementedException();
    //}

    public bool IsListeningToValues => ValuesListeningToKeysSubscription != null;

    private void StartListeningToAllKeys()
    {
        listeningAll_KeysSubscription = Keys.Connect().Subscribe();

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
                                    .Subscribe(v => OnValue(key, v))));
                                if (!values.Lookup(change.Key).HasValue)
                                {
                                    // Add an initial placeholder in the values collection, even though we haven't loaded the value yet. GetValueObservable should load it imminently.
                                    values.AddOrUpdate((change.Key, Optional<TValue>.None));
                                    if (IsListeningToValues)
                                    {
                                        // TODO: Queue load?
                                    }
                                }
                            }
                            break;
                        case ChangeReason.Remove:
                            var existing = su.Lookup(change.Key);
                            if (existing.HasValue)
                            {
                                existing.Value.subscription.Dispose();
                                su.RemoveKey(change.Key);
                                values.Remove(change.Key);
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

    IDisposable? ValuesListeningToKeysSubscription;
    private void StopListeningToAllValues()
    {
        ValuesListeningToKeysSubscription?.Dispose();
        ValuesListeningToKeysSubscription = null;
    }

    private void StopListeningToAllKeys()
    {
        listeningAll_KeysSubscription?.Dispose();
        listeningAll_KeysSubscription = null;

        listeningAllSubscription?.Dispose();
        listeningAllSubscription = null;

        listenToAllListeners.Items.ToList().ForEach(i => i.subscription.Dispose());
        listenToAllListeners.Clear();
    }

    protected string GetKeyedFilePath(TKey key) => Path.Combine(Dir.Path ?? throw new ArgumentNullException(), $"{key}.{SecondExtension}{Extension}");
    protected string GetDefaultFilePath(TKey key) => Path.Combine(Dir.Path ?? throw new ArgumentNullException(), key.ToString()!, $"{SecondExtension}{Extension}");

    protected abstract ValueTask<Optional<TValue>> ReadFromFile(string filePath);

    //protected abstract void LoadValues();
    //protected abstract void StopListeningForValues();
}

// Scraps
//public static string ToKebabCase(string input)
//    {
//        // Replace spaces with hyphens and convert to lowercase
//        return Regex.Replace(input, @"\s+", "-").ToLower();
//    }