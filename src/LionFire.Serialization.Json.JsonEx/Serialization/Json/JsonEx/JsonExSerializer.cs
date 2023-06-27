using System;
using System.Collections.Generic;
using LionFire.Dependencies;
using LionFire.Persistence;
using JsonExSerializer;
using JsonExSerializationContext = JsonExSerializer.SerializationContext;
using LionFire.Types;
using Microsoft.Extensions.Options;
using JsonExSerializer.MetaData;

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

        public JsonExSerializationContext DefaultSettings => new JsonExSerializationContext
        {
            IsCompact = false,
            ReferenceWritingType = JsonExSerializationContext.ReferenceOption.ErrorCircularReferences,
            TypeAliases = GetTypeAliases(),
            //TypeHandlerFactory = ,
        };

        #endregion

        private TypeAliasCollection GetTypeAliases()
        {
            var result = new TypeAliasCollection();
            foreach (var r in TypeNameRegistry.CurrentValue.Types)
            {
                result.Add(r.Value, r.Key);
            }
            return result;
        }

        IOptionsMonitor<TypeNameRegistry> TypeNameRegistry;

        public JsonExLionFireSerializer(IOptionsMonitor<TypeNameRegistry> typeNameRegistry)
        {
            TypeNameRegistry = typeNameRegistry;
        }

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
        {
            string typeName = null;
            if (str.StartsWith("("))
            {
                typeName = str.Substring(1, str.IndexOf(")") - 1);
            }
            if (typeof(T) == typeof(object))
            {
                if (TypeNameRegistry.CurrentValue.Types.TryGetValue(typeName, out Type type))
                {
                    var newJson = str.Substring(str.IndexOf(")") + 1);
                    return (T)new global::JsonExSerializer.Serializer(type, DefaultSettings).Deserialize(newJson);
                }
            }
            return (T)new global::JsonExSerializer.Serializer(typeof(T), DefaultSettings).Deserialize(str);
        }
        //=> JsonConvert.DeserializeObject<TValue>(str, Settings);

        #endregion


    }
}
