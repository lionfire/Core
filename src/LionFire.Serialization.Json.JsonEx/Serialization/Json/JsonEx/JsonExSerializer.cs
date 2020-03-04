using System;
using System.Collections.Generic;
using LionFire.Dependencies;
using LionFire.Persistence;
using JsonExSerializer;
using JsonExSerializationContext = JsonExSerializer.SerializationContext;

namespace LionFire.Serialization.Json.JsonEx
{
    //public class JsonExService : ISerializationService
    //{
    //    public IEnumerable<ISerializationStrategy> AllStrategies { get {
    //            yield return new JsonExSerializer();
    //        } }
    //}

    public class JsonExLionFireSerializer : SerializerBase<JsonExLionFireSerializer>
    {
        public override SerializationFlags SupportedCapabilities =>
            SerializationFlags.Text
            | SerializationFlags.HumanReadable
            | SerializationFlags.Minify
            | SerializationFlags.Deserialize
            | SerializationFlags.Serialize
            ;

        #region (Static) Default Settings

        public static JsonExSerializationContext DefaultSettings = new JsonExSerializationContext
        {
            IsCompact = false,
            ReferenceWritingType = JsonExSerializationContext.ReferenceOption.ErrorCircularReferences,
            //TypeAliases = ,
            //TypeHandlerFactory = ,
        };

        #endregion

        public override SerializationFormat DefaultFormat => defaultFormat;
        private static readonly SerializationFormat defaultFormat = new SerializationFormat("jsx", "JSON Ex", "application/x-json-ex")
        {
            Description = "Javascript Object Notation (Extended)",
        };

        //#region Settings

        //[TryInject]
        //public SerializerSettings Settings
        //{
        //    get => settings ?? DefaultSettings;
        //    set => settings = value;
        //}
        //private SerializerSettings settings;

        //#endregion

        #region Serialize

        public override (string String, SerializationResult Result) ToString(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
            => (new global::JsonExSerializer.Serializer(obj.GetType(), DefaultSettings).Serialize(obj), SerializationResult.Success);
        //=> (JsonConvert.SerializeObject(obj, typeof(object), Settings), SerializationResult.Success);

        #endregion

        #region Deserialize

        public override DeserializationResult<T> ToObject<T>(string str, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
            => (T)new global::JsonExSerializer.Serializer(typeof(T), DefaultSettings).Deserialize(str);
        //=> JsonConvert.DeserializeObject<T>(str, Settings);

        #endregion


    }
}
