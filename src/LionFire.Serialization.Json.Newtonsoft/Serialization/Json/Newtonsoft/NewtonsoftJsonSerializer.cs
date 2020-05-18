using System;
using System.Collections.Generic;
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

        public NewtonsoftJsonSerializer(IOptionsMonitor<JsonSerializerSettings> optionsMonitor)
        {
            PersistenceSettings = optionsMonitor.Get(LionSerializeContext.Persistence.ToString());
            NetworkSettings = optionsMonitor.Get(LionSerializeContext.Network.ToString());
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

        #region Serialize

        public override (string String, SerializationResult Result) ToString(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null) => (JsonConvert.SerializeObject(obj, typeof(object), SettingsForContext(operation)), SerializationResult.Success);

        #endregion

        #region Deserialize

        public override DeserializationResult<T> ToObject<T>(string str, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
            => JsonConvert.DeserializeObject<T>(str, SettingsForContext(operation));

        #endregion


    }
}
