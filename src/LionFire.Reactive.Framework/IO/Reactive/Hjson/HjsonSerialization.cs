using Hjson;
using LionFire.Structures;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using System.Text;
//using System.Text.Json;
using JsonValue = Hjson.JsonValue;

namespace LionFire.IO.Reactive.Hjson;

public static class HjsonSerialization
{
    static JsonSerializerSettings Settings
    {
        get
        {
            if (settings == null)
            {
                settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    //ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                settings.ContractResolver = Singleton<IgnoreDataContractContractResolver>.Instance; // MEMORY: permanently increases with seen types

                //settings.ContractResolver = new DefaultContractResolver
                //{
                //    NamingStrategy = new DefaultNamingStrategy()
                //    //NamingStrategy = new CamelCaseNamingStrategy
                //    //{
                //    //    ProcessDictionaryKeys = true,
                //    //    OverrideSpecifiedNames = false
                //    //}
                //};
            }
            return settings;
        }
    }

    static JsonSerializerSettings? settings;

    public static TValue Deserialize<TValue>(byte[] underlying)
    {
        var hjson = Encoding.UTF8.GetString(underlying);
        var json = HjsonValue.Parse(hjson).ToString(Stringify.Plain);

        return Newtonsoft.Json.JsonConvert.DeserializeObject<TValue>(json
            , Settings
            )!;
        //return JsonSerializer.Deserialize<TValue>(json) ?? throw new NotSupportedException("Deserializing null not supported");
    }

    public static byte[] Serialize<TValue>(TValue usable)
    {
        //var json = JsonSerializer.Serialize(usable);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(usable, Settings);
        var hjson = JsonValue.Parse(json).ToString(new HjsonOptions { EmitRootBraces = false });
        return Encoding.UTF8.GetBytes(hjson);
    }
}

