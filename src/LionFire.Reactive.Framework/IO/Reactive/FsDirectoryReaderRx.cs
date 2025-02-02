using DynamicData.Binding;
using LionFire.IO.Reactive.Filesystem;
using LionFire.IO.Reactive.Hjson;
using LionFire.Reactive.Persistence;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace LionFire.IO.Reactive;

public abstract class FsDirectoryReaderRx<TKey, TValue> : IObservableReader<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    protected abstract string Extension { get; }

    #region Parameters

    public DirectorySelector Dir { get; }

    #endregion

    #region Lifecycle

    public FsDirectoryReaderRx(DirectorySelector dir)
    {
        Dir = dir;
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
        KeyedItems = keys.ToObservableChangeSet()
            .Transform(key => new KeyValuePair<TKey, TValue>(key, ReadFromFile(GetFilePath(key))!))
            .AddKey(kvp => kvp.Key)
            //.Flatten()
            //.Where(change => change.Current.Value != null)
            .AsObservableCache()
            ;
        ObservableCache = KeyedItems.Connect().Transform(x => x.Value).AsObservableCache();


    }

    #endregion

    #region State

    public IObservableList<TKey> Keys => keys.ToObservableChangeSet().AsObservableList();
    private readonly ObservableCollection<TKey> keys = new();

    public IObservableCache<KeyValuePair<TKey, TValue>, TKey> KeyedItems { get; }
    public IObservableCache<TValue, TKey> ObservableCache { get; }


    #endregion

    protected async ValueTask LoadKeys()
    {
        await Task.Run(() =>
        {
            if (Directory.Exists(Dir.Path))
            {
                var files = Directory.GetFiles(Dir.Path, "*" + Extension, new EnumerationOptions { RecurseSubdirectories = Dir.Recursive });
                foreach (var file in files)
                {
                    var key = Path.GetFileNameWithoutExtension(file);
                    if (key is TKey typedKey)
                    {
                        keys.Add(typedKey);
                    }
                }
            }
        });
    }

    public abstract IObservable<TValue?> Listen(TKey key);

    protected string GetFilePath(TKey key) => Path.Combine(Dir.Path ?? throw new ArgumentNullException(), $"{key}{Extension}");

    protected abstract TValue? ReadFromFile(string filePath);

}

