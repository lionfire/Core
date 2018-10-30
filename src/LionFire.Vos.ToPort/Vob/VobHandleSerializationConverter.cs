using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonExSerializer.TypeConversion;

namespace LionFire.Vos
{
    public class VobHandleSerializationConverter : IJsonTypeConverter
    {
        public object Context
        {
            set { }
        }

        public object ConvertFrom(object item, JsonExSerializer.SerializationContext serializationContext)
        {
            IVobHandle assetID = (IVobHandle)item;
            return assetID.Path;
        }

        public object ConvertTo(object item, Type sourceType, JsonExSerializer.SerializationContext serializationContext)
        {
            IVobHandle vh = (IVobHandle) Activator.CreateInstance(sourceType);
            vh.Path = (string)item;
            return vh;
        }

        public Type GetSerializedType(Type sourceType)
        {
            return typeof(string);
        }

        public bool SupportsReferences(Type sourceType, JsonExSerializer.SerializationContext serializationContext)
        {
            return false;
        }
    }    
}
