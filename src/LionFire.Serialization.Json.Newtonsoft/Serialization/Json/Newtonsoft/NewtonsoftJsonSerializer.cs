using LionFire.DependencyInjection;
using LionFire.Serialization.Contexts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Serialization.Json.Newtonsoft
{
    public class NewtonsoftJsonSerializer : SerializerBase
    {
        public override SerializationFlags SupportedCapabilities =>
            SerializationFlags.Text
            | SerializationFlags.HumanReadable
            | SerializationFlags.Minify
            | SerializationFlags.Deserialize
            | SerializationFlags.Serialize
            ;

        #region Static

        public static JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            //Converters = ,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            //NullValueHandling = 
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        };

        #endregion

        public override IEnumerable<string> FileExtensions
        {
            get
            {
                base.FileExtensions
                yield return "json";
            }
        }

        public override IEnumerable<string> MimeTypes
        {
            get
            {
                yield return "application/json";
            }
        }

        #region Settings

        [TryInject]
        public JsonSerializerSettings Settings
        {
            get { return settings ?? DefaultSettings; }
            set { settings = value; }
        }

        
        

        public override SerializationFormat DefaultFormat => throw new NotImplementedException();

        private JsonSerializerSettings  settings;
        
        #endregion

        #region Serialize

        public override string ToString(SerializationContext context)
        {
            return JsonConvert.SerializeObject(context.Object, typeof(object), Settings);
        }

        #endregion

        #region Deserialize

        public override T ToObject<T>(SerializationContext context = null)
        {
            context.LoadStringDataIfNeeded();
            return JsonConvert.DeserializeObject<T>(context.StringData, Settings);
        }

        #endregion

        
    }
}
