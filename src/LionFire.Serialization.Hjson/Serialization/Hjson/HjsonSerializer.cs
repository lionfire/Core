#nullable enable 
using System;
using System.Collections.Generic;
using LionFire.Dependencies;
using LionFire.Persistence;
using Microsoft.Extensions.Options;
using Hjson;

namespace LionFire.Serialization.Hjson_;

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

        if (obj is not string jsonObj) { throw new ArgumentException("Can only serialize strings"); }
        var hjsonString = JsonValue.Parse(jsonObj).ToString(Stringify.Hjson);
        return (hjsonString, SerializationResult.Success);
    }

    #endregion

    #region Deserialize

    public override DeserializationResult<T> ToObject<T>(string str, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
    {
        if (typeof(T) != typeof(string)) throw new ArgumentException("Can only deserialize to string");

        var jsonString = HjsonValue.Parse(str).ToString();
        return (DeserializationResult<T>)(object)new DeserializationResult<string>(jsonString); // hardcast

        //var settings = SettingsForContext(operation); // TODO

        //var obj = JsonConvert.DeserializeObject<T>(str);  // HARDCODE - Newtonsoft.Json
        //return new DeserializationResult<T>(obj);
    }

    #endregion

    public override IEnumerable<SerializationFormat>? SupportedInputs => supportedInputs;
    private static readonly IEnumerable<SerializationFormat> supportedInputs = [Json];

    static SerializationFormat Json => new SerializationFormat("json", "JSON", "application/json");
}
