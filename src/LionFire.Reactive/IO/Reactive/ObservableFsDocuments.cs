using DynamicData;
using LionFire.IO.Reactive;
using LionFire.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Data.Filesystem;

public class FsWatchOptions
{
    public string? SearchPattern { get; set; }

    //public EnumerationOptions? EnumerationOptions { get; set; }

    //public bool ReadOnly { get; set; }
}

public static class ObservableFsDocuments
{
    public static IObservable<IChangeSet<(string key, TValue value), string>> Create<TValue>(string dir, Func<byte[], TValue> deserialize, FsWatchOptions? options = null)
        where TValue : notnull
    {
        return PollOnDemand(dir, deserialize, options?.SearchPattern);
    }

    public static IObservable<IChangeSet<(string key, TValue value), string>> PollOnDemand<TValue>(string dir, Func<byte[], TValue> deserialize, string? searchPattern = null)
    {
        return ObservableEx2.CreateConnectOnDemand(x => x.key,
            resourceFactory: resourceFactory(dir, deserialize, searchPattern)
        );
    }

    private static Func<SourceCache<(string key, TValue value), string>, IDisposable> resourceFactory<TValue>(string dir, Func<byte[], TValue> deserialize, string? searchPattern)
    {
        return cache =>
        {
            var fileInfos = ObservableFileInfos.PollOnDemand(dir, searchPattern).AsObservableCache();

            fileInfos.Connect().Subscribe(fi =>
            {
                var additionsAndUpdates = fi.Where(change => change.Reason != ChangeReason.Remove).Select(fileInfo =>
                {
                    var key = fileInfo.Key;
                    var value = deserialize(File.ReadAllBytes(fileInfo.Current.FullName));
                    return (key, value);
                });
                cache.Edit(updater =>
                {
                    updater.RemoveKeys(fi.Where(change => change.Reason == ChangeReason.Remove).Select(change => change.Current.Name));
                    updater.AddOrUpdate(additionsAndUpdates);
                });
            });

            return fileInfos;
        };
    }
}

// ENH: FilesystemWatcher instead of polling
public class ObservableFsDocuments<TValue>
    where TValue : notnull
{

    public IObservableCache<(string name, TValue value), string> ObservableCache => sourceCache.AsObservableCache();
    public SourceCache<(string name, TValue value), string> SourceCache => sourceCache;
    private SourceCache<(string name, TValue value), string> sourceCache;

    public ObservableFsDocuments(string dir, Func<string, ValueTask<TValue>> deserializeFile, FsWatchOptions? options = null)
    {
        sourceCache = new SourceCache<(string name, TValue value), string>(x => x.name);
        var fileInfos = ObservableFileInfos.PollOnDemand(dir, options?.SearchPattern);

        fileInfos.Subscribe(changes =>
        {
            var updates = changes.Where(change => change.Reason == ChangeReason.Add || change.Reason == ChangeReason.Update).Select(change =>
            new
            {
                change.Current.Name,
                Value = deserializeFile(change.Current.FullName).Result // AsyncToSync
            });

            sourceCache.Edit(updater =>
            {
                updater.RemoveKeys(changes.Where(change => change.Reason == ChangeReason.Remove).Select(change => change.Current.Name));

                updater.AddOrUpdate(updates.Select(u => (u.Name, u.Value)));

                foreach (var update in updates)
                {
                    sourceCache.Edit(u => u.AddOrUpdate((update.Name, update.Value)));
                }
            });
        });
    }

}
