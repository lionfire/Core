using DynamicData;
using Hjson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LionFire.Data.Collections.Filesystem;

public abstract class SerializingFileCollection<TValue> : AsyncTransformingDictionary<string, byte[], TValue>
    where TValue : notnull
{
    public string Dir => Underlying.Dir;

    public new AsyncFileDictionary Underlying => (AsyncFileDictionary)base.Underlying;
    //protected override Func<(string name, byte[]), string> KeySelector => x => x.Key;

    public SerializingFileCollection(string dir) : base(new AsyncFileDictionary(dir))
    {
    }
}

public class JsonFileCollection<TValue> : SerializingFileCollection<TValue>
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
public class HjsonFileCollection<TValue> : SerializingFileCollection<TValue>
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



