using System;
using System.Collections.Generic;
using LionFire.Dependencies;
using LionFire.Persistence;
using Microsoft.Extensions.Options;
using Hjson;
using Newtonsoft.Json;

namespace LionFire.Serialization.Hjson;

public class HjsonSerializer : SerializerBase<HjsonSerializer>
{
    public override SerializationFlags SupportedCapabilities =>
        SerializationFlags.Text
        | SerializationFlags.HumanReadable
        | SerializationFlags.Deserialize
        | SerializationFlags.Serialize
        ;

    public HjsonSerializer(IOptionsMonitor<HjsonSerializerSettings> optionsMonitor)
    {
        PersistenceSettings = optionsMonitor.Get(LionSerializeContext.Persistence.ToString());
        NetworkSettings = optionsMonitor.Get(LionSerializeContext.Network.ToString());
    }

    public override SerializationFormat DefaultFormat => defaultFormat;
    private static readonly SerializationFormat defaultFormat = new SerializationFormat("hjson", "Hjson", "application/hjson")
    {
        Description = "Human JSON",
    };

    #region Settings

    public HjsonSerializerSettings PersistenceSettings { get; }
    public HjsonSerializerSettings NetworkSettings { get; }

    public HjsonSerializerSettings SettingsForContext(PersistenceOperation op)
    {
        LionSerializeContext lionSerializeContext = op.SerializeContext;

        Exception ThrowMultiple() => new ArgumentException("LionSerializeContext can only be a single flag");

        return lionSerializeContext switch
        {
            LionSerializeContext.Persistence => PersistenceSettings,
            LionSerializeContext.Network => NetworkSettings,
            LionSerializeContext.Copy => NetworkSettings,

            LionSerializeContext.AllSerialization => throw ThrowMultiple(),
            LionSerializeContext.All => throw ThrowMultiple(),
            //LionSerializeContext.None => throw new NotImplementedException(),
            _ => throw new ArgumentException("LionSerializeContext is not set"),
        };
    }
    #endregion

    #region Serialize

    public override (string String, SerializationResult Result) ToString(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
    {
        // SettingsForContext(operation) // TODO
        var json = JsonConvert.SerializeObject(obj); // HARDCODE - Newtonsoft.Json
        var hjsonString = JsonValue.Parse(json).ToString(Stringify.Hjson);
        return (hjsonString, SerializationResult.Success);
    }

    #endregion

    #region Deserialize

    public override DeserializationResult<T> ToObject<T>(string str, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
    {
        var jsonString = HjsonValue.Parse(str).ToString();

        //var settings = SettingsForContext(operation); // TODO

        var obj = JsonConvert.DeserializeObject<T>(str);  // HARDCODE - Newtonsoft.Json
        return new DeserializationResult<T>(obj);
    }

    #endregion


}
