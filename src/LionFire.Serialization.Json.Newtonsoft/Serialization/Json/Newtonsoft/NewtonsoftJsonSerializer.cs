using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using JsonKnownTypes;
using LionFire.Dependencies;
using LionFire.Persistence;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LionFire.Serialization.Json.Newtonsoft
{
    //public class NewtonsoftJsonService : ISerializationService
    //{
    //    public IEnumerable<ISerializationStrategy> AllStrategies { get {
    //            yield return new NewtonsoftJsonSerializer();
    //        } }
    //}

    // UPSTREAM - Concurrency issue: https://github.com/JamesNK/Newtonsoft.Json/issues/1452

    // OPTIMIZE - keep AsyncLocal serializers for each possibility of SettingsForContext()?

    public class NewtonsoftJsonSerializer : SerializerBase<NewtonsoftJsonSerializer>
    {
        public override SerializationFlags SupportedCapabilities =>
            SerializationFlags.Text
            | SerializationFlags.HumanReadable
            | SerializationFlags.Minify
            | SerializationFlags.Deserialize
            | SerializationFlags.Serialize
            ;

        //#region (Static) Default Settings

        //public static JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        //{
        //    TypeNameHandling = TypeNameHandling.Auto,
        //    //Converters = ,
        //    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
        //    //NullValueHandling = 
        //    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        //};

        //#endregion

        public NewtonsoftJsonSerializer(IOptionsMonitor<JsonSerializerSettings> optionsMonitor, IServiceProvider serviceProvider)
        {
            PersistenceSettings = optionsMonitor.Get(LionSerializeContext.Persistence.ToString());
            NetworkSettings = optionsMonitor.Get(LionSerializeContext.Network.ToString());

            if (serviceProvider.GetService(typeof(KnownTypesBinder)) is KnownTypesBinder knownTypesBinder)
            {
                PersistenceSettings.SerializationBinder = knownTypesBinder;
                NetworkSettings.SerializationBinder = knownTypesBinder;
            }
        }

        public override SerializationFormat DefaultFormat => defaultFormat;
        private static readonly SerializationFormat defaultFormat = new SerializationFormat("json", "JSON", "application/json")
        {
            Description = "Javascript Object Notation",
        };

        #region Settings

        public JsonSerializerSettings PersistenceSettings { get; }
        public JsonSerializerSettings NetworkSettings { get; }

        public JsonSerializerSettings SettingsForContext(PersistenceOperation op)
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

        #region Serializers

#if Experimental
        ConcurrentDictionary<LionSerializeContext, AsyncLocal<JsonConverter>> converters = new ConcurrentDictionary<LionSerializeContext, AsyncLocal<JsonSerializer>>();
        public JsonConverter GetConverter(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            var sc = operation?.Value?.SerializeContext;
            var local = serializers.GetOrAdd(operation.Value.SerializeContext, _ => new AsyncLocal<JsonConverter>());
            return local.Value ??= new LionFireJsonConverter();
        }

#endif
        //ConcurrentDictionary<LionSerializeContext, AsyncLocal<JsonSerializer>> serializers = new ConcurrentDictionary<LionSerializeContext, AsyncLocal<JsonSerializer>>();
        //public JsonSerializer GetSerializer(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        //{
        //    var sc = operation?.Value?.SerializeContext;
        //    var local = serializers.GetOrAdd(operation.Value.SerializeContext, _ => new AsyncLocal<JsonSerializer>());
        //    return local.Value ??= JsonSerializer.Create(SettingsForContext(operation));
        //}

        #endregion

        #region Serialize

        //public override (string String, SerializationResult Result) ToString(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null) => GetSerializer().Serialize(JsonConvert.SerializeObject(obj, typeof(object), SettingsForContext(operation)), SerializationResult.Success);
        public override (string String, SerializationResult Result) ToString(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null) => (JsonConvert.SerializeObject(obj, typeof(object), SettingsForContext(operation)), SerializationResult.Success);

        #endregion

        #region Deserialize

        public override DeserializationResult<T> ToObject<T>(string str, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
            => JsonConvert.DeserializeObject<T>(str, SettingsForContext(operation));

        #endregion

    }
}
