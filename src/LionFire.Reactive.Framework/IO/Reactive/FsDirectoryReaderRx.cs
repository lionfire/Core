// ENH maybe: for now just support the one approach of nameless documents.  Uncommenting this would allow for the document file to carry the matching name of the parent directory, which may make for more ease of use in some situations (such as having multiple files open in an editor.)
//#define DocumentWithMatchingKeyName
using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;
using LionFire.IO.Reactive.Filesystem;
using LionFire.Persistence.Filesystemlike;
using LionFire.Reactive.Persistence;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LionFire.IO.Reactive;

public abstract class FsDirectoryReaderRx<TKey, TValue> : IObservableReader<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    protected abstract string Extension { get; }
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

    public FsDirectoryReaderRx(DirectorySelector dir, IFileExtensionConvention extensionConvention)
    {
        Dir = dir;
        ExtensionConvention = extensionConvention;
        _ = LoadKeys();

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
        KeyedItems = keys.Connect()
            .Transform(key => new KeyValuePair<TKey, TValue>(key, ReadFromFile(GetFilePath(key))!))
            //.AddKey(kvp => kvp.Key)
            //.Flatten()
            //.Where(change => change.Current.Value != null)
            .AsObservableCache()
            ;
        ObservableCache = KeyedItems.Connect().Transform(x => x.Value).AsObservableCache();


    }

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
    private readonly SourceCache<TKey, TKey> keys = new(k => k);

    public IObservableCache<KeyValuePair<TKey, TValue>, TKey> KeyedItems { get; }
    public IObservableCache<TValue, TKey> ObservableCache { get; }


    #endregion

    //protected virtual GetKeyForFile(string path)
    //{
    //}

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
        await Task.Run(() =>
        {
            var namelessDocumentFile = SecondExtension + Extension;

            if (Directory.Exists(Dir.Path))
            {
                var dirName = Path.GetDirectoryName(Dir.Path) ?? RootDirName;

                var files = Directory.GetFiles(Dir.Path, "*" + SecondExtension + Extension, new EnumerationOptions { RecurseSubdirectories = Dir.Recursive });
                bool isStringKey = typeof(TKey) == typeof(string);

                bool namelessDocumentFileExists = files.Contains(namelessDocumentFile); // Takes precedence
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

    public abstract IObservable<TValue?> Listen(TKey key);

    protected string GetFilePath(TKey key) => Path.Combine(Dir.Path ?? throw new ArgumentNullException(), $"{key}{Extension}");

    protected abstract TValue? ReadFromFile(string filePath);

}

