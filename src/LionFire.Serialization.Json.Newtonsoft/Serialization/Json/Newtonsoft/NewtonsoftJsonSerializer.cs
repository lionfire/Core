using LionFire.Serialization.Contexts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Serialization.Json.Newtonsoft
{
    public class NewtonsoftJsonSerializer : StringSerializerBase
    {

        public override IEnumerable<string> FileExtensions
        {
            get
            {
                yield return ".json";
            }
        }
        public override IEnumerable<string> MimeTypes
        {
            get
            {
                yield return "application/json";
            }
        }

        #region Serialize


        public override string ToString(object obj, SerializationContext context = null)
        {
            return JsonConvert.SerializeObject(obj);
        }


        #endregion

        #region Deserialize
        
        public override T ToObject<T>(string serializedData, SerializationContext context = null)
        {
            return JsonConvert.DeserializeObject<T>(serializedData);
        }

        #endregion
    }
}
