using DynamicData;
using LionFire.IO.Reactive;
using System;
using System.Collections.Generic;
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

// ENH: FilesystemWatcher instead of polling
public class DocumentFsWatcher<TValue>
    where TValue : notnull
{

    public IObservableCache<(string name, TValue value), string> ObservableCache => sourceCache.AsObservableCache();
    public SourceCache<(string name, TValue value), string> SourceCache => sourceCache;
    private SourceCache<(string name, TValue value), string> sourceCache;

    public DocumentFsWatcher(string dir, Func<string, Func<string, ValueTask<TValue>>> deserializeFile, FsWatchOptions? options = null)
    {
        sourceCache = new SourceCache<(string name, TValue value), string>(x => x.name);
        var fileInfos = FileInfoObservable.PollOnDemand(dir, options?.SearchPattern);

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
