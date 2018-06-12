using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LionFire.Serialization.JsonEx
{
    //public class SerializeAsConverter : IJsonTypeConverter
    //{
    //    public object Context
    //    {
    //        set { }
    //    }

    //    public object ConvertFrom(object item, SerializationContext serializationContext)
    //    {
    //        return ((ISerializeAs)item).SerializedValue;
    //    }

    //    public object ConvertTo(object item, Type sourceType, SerializationContext serializationContext)
    //    {
    //        return ((ISerializeAs)item).SerializedValue;
    //    }

    //    public Type GetSerializedType(Type sourceType)
    //    {
    //        // REVIEW - is this a problem?
    //        return typeof(object);
    //    }

    //    public bool SupportsReferences(Type sourceType, SerializationContext serializationContext)
    //    {
    //        return false;
    //    }
    //}

    //public class SerializeAsProcessor : AttributeProcessor
    //{
    //    public override void Process(IMetaData metaData, ICustomAttributeProvider attributeProvider, IConfiguration config)
    //    {
    //        if (metaData is IPropertyData)
    //        {
    //            IPropertyData property = (IPropertyData)metaData;
    //            if(typeof(ISerializeAs).IsAssignableFrom(property.PropertyType))
    //            {
    //                property.TypeConverter = new SerializeAsConverter(property.FromType, 
    //            }

    //            if (attributeProvider.IsDefined(typeof(SerializeAsValueAttribute), false))
    //            {
    //                var attrs = attributeProvider.GetCustomAttributes(typeof(SerializeAsValueAttribute), false);

    //                foreach (var attr in attrs.OfType<SerializeAsValueAttribute>())
    //                {
    //                    if(attr.SerializeAsValue.HasValue)
    //                    {
    //                        property.DefaultValueSetting = attr.SerializeAsValue.Value ? DefaultValueOption.WriteAllValues : DefaultValueOption.SuppressDefaultValues;
    //                        property.TypeConverter =
    //                        break;
    //                    }
    //                }                    
    //            }
    //        }
    //    }
    //}


    
}
