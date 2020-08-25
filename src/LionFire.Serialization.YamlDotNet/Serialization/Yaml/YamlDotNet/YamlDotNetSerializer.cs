using System;
using System.Collections.Generic;
using LionFire.Dependencies;
using LionFire.Persistence;
using Microsoft.Extensions.Options;
using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LionFire.Serialization.Yaml.YamlDotNet
{
    //public class YamlDotNetService : ISerializationService
    //{
    //    public IEnumerable<ISerializationStrategy> AllStrategies { get {
    //            yield return new YamlDotNetSerializer();
    //        } }
    //}

    // UPSTREAM - Concurrency issue: https://github.com/JamesNK/Newtonsoft.Yaml/issues/1452

    public class YamlDotNetOptions
    {
        public Dictionary<string, Type> TypeMappings = new Dictionary<string, Type>();
    }

    public class YamlDotNetSerializer : SerializerBase<YamlDotNetSerializer>
    {
        public override SerializationFlags SupportedCapabilities =>
            SerializationFlags.Text
            | SerializationFlags.HumanReadable
            | SerializationFlags.Deserialize
            | SerializationFlags.Serialize
            ;

        IOptionsMonitor<YamlDotNetOptions> optionsMonitor;

        //#region (Static) Default Settings

        //public static YamlSerializerSettings DefaultSettings = new YamlSerializerSettings
        //{
        //    TypeNameHandling = TypeNameHandling.Auto,
        //    //Converters = ,
        //    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
        //    //NullValueHandling = 
        //    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        //};

        //#endregion


        public YamlDotNetSerializer(IOptionsMonitor<YamlDotNetOptions> optionsMonitor)
        {
            this.optionsMonitor = optionsMonitor;
            //PersistenceSettings = optionsMonitor.Get(LionSerializeContext.Persistence.ToString());
            //NetworkSettings = optionsMonitor.Get(LionSerializeContext.Network.ToString());
        }

        public override SerializationFormat DefaultFormat => defaultFormat;

        //MIME type: see https://stackoverflow.com/questions/332129/yaml-mime-type
        private static readonly SerializationFormat defaultFormat = new SerializationFormat("yaml", "YAML", "application/x-yaml")
        {
            Description = "YAML",
        };

        #region Settings

        //public YamlSerializerSettings PersistenceSettings { get; }
        //public YamlSerializerSettings NetworkSettings { get; }

        //public YamlSerializerSettings SettingsForContext(PersistenceOperation op)
        //{
        //    LionSerializeContext lionSerializeContext = op.SerializeContext;

        //    Exception ThrowMultiple() => new ArgumentException("LionSerializeContext can only be a single flag");

        //    return lionSerializeContext switch
        //    {
        //        LionSerializeContext.Persistence => PersistenceSettings,
        //        LionSerializeContext.Network => NetworkSettings,
        //        LionSerializeContext.Copy => NetworkSettings,

        //        LionSerializeContext.AllSerialization => throw ThrowMultiple(),
        //        LionSerializeContext.All => throw ThrowMultiple(),
        //        //LionSerializeContext.None => throw new NotImplementedException(),
        //        _ => throw new ArgumentException("LionSerializeContext is not set"),
        //    };
        //}
        #endregion

        #region Serialize
        
        public override (string String, SerializationResult Result) ToString(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            //(YamlConvert.SerializeObject(obj, typeof(object), SettingsForContext(operation)), SerializationResult.Success);

            var serializer = new SerializerBuilder()
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
                .AddTagMappings(optionsMonitor.CurrentValue)
                .EnsureRoundtrip()
                .Build();
            var yaml = serializer.Serialize(obj);
            return (yaml, SerializationResult.Success);
        }

        #endregion

        #region Deserialize

        public override DeserializationResult<T> ToObject<T>(string str, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            var deserializer = new DeserializerBuilder()
                .AddTagMappings(optionsMonitor.CurrentValue)
                .Build();
            var order = deserializer.Deserialize<T>(str);
            return order;
        }

        #endregion

    }
    internal static class YamlDotNetInternalExtensions
    {
        public static SerializerBuilder AddTagMappings(this SerializerBuilder builder, YamlDotNetOptions options)
        {
            foreach(var m in options.TypeMappings)
            {
                builder.WithTagMapping(m.Key, m.Value);
            }
            return builder;
        }
        public static DeserializerBuilder AddTagMappings(this DeserializerBuilder builder, YamlDotNetOptions options)
        {
            foreach (var m in options.TypeMappings)
            {
                builder.WithTagMapping(m.Key, m.Value);
            }
            return builder;
        }
    }
}
