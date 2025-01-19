using DynamicData;
using System;
using System.Threading.Tasks;

namespace LionFire.Data.Filesystem;

public class DocumentFsRepository<TValue>
    where TValue : notnull
{
    ObservableFsDocuments<TValue> documentFsWatcher;

    public IObservableCache<(string name, TValue value), string> ObservableCache => documentFsWatcher.ObservableCache;

    public DocumentFsRepository(string dir, Func<string, TValue, ValueTask> serializeFile,  Func<string, ValueTask<TValue>> deserializeFile, FsWatchOptions? watchOptions = null)
    {
        documentFsWatcher = new ObservableFsDocuments<TValue>(dir, deserializeFile, watchOptions);
        SerializeFile = serializeFile;
    }

    public Func<string, TValue, ValueTask> SerializeFile { get; }

    public async ValueTask Save(string name, TValue value)
    {
        var old = ObservableCache.Lookup(name);
        try
        {
            documentFsWatcher.SourceCache.AddOrUpdate((name, value));
            await SerializeFile(name, value);
        }
        catch
        {
            if (old.HasValue)
            {
                documentFsWatcher.SourceCache.AddOrUpdate(old.Value);
            }
            throw;
        }
    }

}
