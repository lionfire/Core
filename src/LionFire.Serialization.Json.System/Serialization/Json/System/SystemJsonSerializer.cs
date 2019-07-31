using System;
using System.Collections.Generic;
using LionFire.DependencyInjection;
using LionFire.Persistence;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LionFire.Serialization.Json.System
{
    
    public class SystemJsonSerializer : SerializerBase<SystemJsonSerializer>
    {
        public override SerializationFlags SupportedCapabilities =>
            SerializationFlags.Text
            | SerializationFlags.HumanReadable
            | SerializationFlags.Minify
            | SerializationFlags.Deserialize
            | SerializationFlags.Serialize
            ;

        #region (Static) Default Settings

        public static JsonSerializerOptions DefaultSerializeSettings = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            //Converters = ,
            //DefaultBufferSize = ,
            WriteIndented = true,
            PropertyNameCaseInsensitive = false,
            IgnoreReadOnlyProperties = true,
            //IgnoreNullValues = ,
            //PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //ReadCommentHandling = JsonCommentHandling.Allow,
        };
        public static JsonSerializerOptions DefaultDeserializeSettings = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            //Converters = ,
            //DefaultBufferSize = ,
            WriteIndented = true,
            PropertyNameCaseInsensitive = false,
            IgnoreReadOnlyProperties = true,
            //IgnoreNullValues = ,
            //PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        #endregion

        public override SerializationFormat DefaultFormat => defaultFormat;
        private static readonly SerializationFormat defaultFormat = new SerializationFormat("json", "JSON", "application/json")
        {
            Description = "Javascript Object Notation",
        };

        #region Settings

        [TryInject]
        public JsonSerializerOptions SerializeSettings
        {
            get => serializeSettings ?? DefaultSerializeSettings;
            set => serializeSettings = value;
        }
        private JsonSerializerOptions serializeSettings;

        [TryInject]
        public JsonSerializerOptions DeserializeSettings
        {
            get => deserializeSettings ?? DefaultDeserializeSettings;
            set => deserializeSettings = value;
        }
        private JsonSerializerOptions deserializeSettings;

        #endregion

        #region Serialize

        public override (string String, SerializationResult Result) ToString(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null) =>
        (JsonSerializer.Serialize<object>(obj, SerializeSettings), SerializationResult.Success);

        #endregion

        #region Deserialize

        public override (T Object, SerializationResult Result) ToObject<T>(string str, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null) => (JsonSerializer.Deserialize<T>(str, DeserializeSettings), SerializationResult.Success);

        #endregion

    }
}
