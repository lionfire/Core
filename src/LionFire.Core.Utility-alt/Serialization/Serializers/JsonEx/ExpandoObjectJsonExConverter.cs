#if TOMIGRATE // Newtonsoft Json
#if !NET35
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonExSerializer.TypeConversion;
using LionFire.Serialization;
using System.Dynamic;

//namespace LionFire.Valor.Serialization

namespace LionFire.Serialization.JsonEx
{
    public class ExpandoObjectJsonExConverter : IJsonTypeConverter
    {
        public object Context
        {
            set { }
        }

        public object ConvertFrom(object item, JsonExSerializer.SerializationContext serializationContext)
        {
            ExpandoObject actualValue = item as ExpandoObject;
            if (actualValue==null) return null;

            Dictionary<string, object> dict = new Dictionary<string, object>();

            foreach(var kvp in (IDictionary<string, object>)actualValue)
            {
                dict.Add(kvp.Key, kvp.Value);
            }

            return dict;
        }

        public object ConvertTo(object item, Type sourceType, JsonExSerializer.SerializationContext serializationContext)
        {
            Dictionary<string, object> serializedValue = (Dictionary<string, object>)item;
            if (serializedValue == null) return null;

            ExpandoObject eo = new ExpandoObject();
           IDictionary<string, object> dict = (IDictionary<string, object>)eo;

            foreach(var kvp in serializedValue)
            {
                dict.Add(kvp.Key, kvp.Value);
            }
            return eo;
        }

        public Type GetSerializedType(Type sourceType)
        {
            return typeof(Dictionary<string, object>);
        }

        public bool SupportsReferences(Type sourceType, JsonExSerializer.SerializationContext serializationContext)
        {
            return false;
        }
    }
}
#endif
#endif