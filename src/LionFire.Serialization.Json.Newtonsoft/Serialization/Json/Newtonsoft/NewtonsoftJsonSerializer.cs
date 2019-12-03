using System;
using System.Collections.Generic;
using LionFire.Dependencies;
using LionFire.Persistence;
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

        #region (Static) Default Settings

        public static JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            //Converters = ,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            //NullValueHandling = 
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        };

        #endregion

        public override SerializationFormat DefaultFormat => defaultFormat;
        private static readonly SerializationFormat defaultFormat = new SerializationFormat("json", "JSON", "application/json")
        {
            Description = "Javascript Object Notation",
        };

        #region Settings

        [TryInject]
        public JsonSerializerSettings Settings
        {
            get => settings ?? DefaultSettings;
            set => settings = value;
        }
        private JsonSerializerSettings settings;

        #endregion

        #region Serialize

        public override (string String, SerializationResult Result) ToString(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null) => (JsonConvert.SerializeObject(obj, typeof(object), Settings), SerializationResult.Success);

        #endregion

        #region Deserialize

        public override DeserializationResult<T> ToObject<T>(string str, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null) 
            => JsonConvert.DeserializeObject<T>(str, Settings);

        #endregion


    }
}
