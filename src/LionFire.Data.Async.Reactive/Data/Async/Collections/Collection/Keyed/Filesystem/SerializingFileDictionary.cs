#if UNUSED
using DynamicData;
using Hjson;
using LionFire.Data.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LionFire.Data.Collections.Filesystem;


public abstract class AsyncReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    protected SourceCache<AsyncValueStatus<TKey, TValue>, TKey> sourceCache = new(s => s.Key);

}

public abstract class SerializingFileDictionary<TValue> : AsyncReadOnlyDictionary<string /* relative path */, TValue>, IDisposable
    //: AsyncTransformingDictionary<string, byte[], TValue>
    where TValue : notnull
{
    #region Parameters

    #endregion

    #region Lifecycle

    CompositeDisposable disposables = new();

    public SerializingFileDictionary(string dir, string? searchPattern = null) : base(dir)
    {
        observableChangeSet = ObservableFsDocuments.Create(dir, Deserialize);
        Items = observableChangeSet.Transform(c => c.value).AsObservableCache();
        disposables.Add(observableChangeSet.Subscribe(OnNext));
    }

    public void Dispose()
    {
        disposables.Dispose();
    }

    #endregion

    public IObservableCache<TValue, string> Items { get; }
    IObservable<IChangeSet<(string key, TValue value), string>> observableChangeSet;


    private void OnNext(IChangeSet<(string key, TValue value), string> set)
    {
        throw new NotImplementedException();
    }

    protected abstract TValue Deserialize(byte[] underlying);
    protected abstract byte[] Serialize(TValue usable);

    public async ValueTask Save(string name, TValue value)
    {
        var old = Items.Lookup(name);
        try
        {
            await Upsert(name, Serialize(value));
            //documentFsWatcher.SourceCache.AddOrUpdate((name, value));
            //await SerializeFile(name, value);
        }
        catch
        {
            if (old.HasValue)
            {
                //documentFsWatcher.SourceCache.AddOrUpdate(old.Value);
            }
            throw;
        }
    }
}

public class JsonFileCollection<TValue> : SerializingFileDictionary<TValue>
    where TValue : notnull
{
    public JsonFileCollection(string dir) : base(dir)
    {
    }

    #region Serialization

    protected override TValue Deserialize(byte[] underlying)
    {
        var json = UTF8Encoding.UTF8.GetString(underlying);
        return JsonSerializer.Deserialize<TValue>(json) ?? throw new NotSupportedException("Deserializing null not supported");
    }


    protected override byte[] Serialize(TValue usable)
    {
        var json = JsonSerializer.Serialize<TValue>(usable);
        return UTF8Encoding.UTF8.GetBytes(json);
    }

    #endregion

}

// MOVE to another DLL and remove Hjson dependency
public class HjsonFileCollection<TValue> : SerializingFileDictionary<TValue>
    where TValue : notnull
{
    public HjsonFileCollection(string dir) : base(dir)
    {
    }

    #region Serialization

    protected override TValue Deserialize(byte[] underlying)
    {
        var hjson = UTF8Encoding.UTF8.GetString(underlying);
        var json = HjsonValue.Parse(hjson).ToString(Stringify.Plain);
        return JsonSerializer.Deserialize<TValue>(json) ?? throw new NotSupportedException("Deserializing null not supported");
    }

    protected override byte[] Serialize(TValue usable)
    {
        var json = JsonSerializer.Serialize<TValue>(usable);
        var hjson = JsonValue.Parse(json).ToString(new HjsonOptions { EmitRootBraces = false });
        return UTF8Encoding.UTF8.GetBytes(json);
    }

    #endregion
}



#endif