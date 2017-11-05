#if TOMIGRATE // Newtonsoft Json
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonExSerializer.TypeConversion;

namespace LionFire.Serialization.Serializers.JsonEx
{
    public class TypeBox
    {
        public string TypeName;
    }

    public class JsonExTypeConverter : IJsonTypeConverter
    {
        public object Context
        {
            set { }
        }

        public object ConvertFrom(object item, JsonExSerializer.SerializationContext serializationContext)
        {
            
            Type type = (Type)item;
            return type.FullName;
            //return new TypeBox { TypeName = type.FullName };
        }

        public object ConvertTo(object item, Type sourceType, JsonExSerializer.SerializationContext serializationContext)
        {
            //TypeBox typeBox = (TypeBox)item;
            //return Type.GetType(typeBox.TypeName);
            string typeFullName = (string)item;
            return Type.GetType(typeFullName);
        }

        public Type GetSerializedType(Type sourceType)
        {
            //return typeof(TypeBox);
            return typeof(string);
        }

        public bool SupportsReferences(Type sourceType, JsonExSerializer.SerializationContext serializationContext)
        {
            return false;
        }
    }
}
#endif