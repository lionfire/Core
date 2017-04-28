#if JsonEx
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonExSerializer.MetaData.Attributes;
using JsonExSerializer.MetaData;
using System.Reflection;
using JsonExSerializer;
using System.ComponentModel;
using System.Collections;

namespace LionFire.Serialization.JsonEx
{
    public class DefaultValueProcessor : AttributeProcessor
    {
        public LionSerializeContext DefaultValueContexts = LionSerializeContext.All;

        public override void Process(IMetaData metaData, ICustomAttributeProvider attributeProvider, IConfiguration config)
        {
            if (metaData is IPropertyData)
            {
                IPropertyData property = (IPropertyData)metaData;
                if (attributeProvider.IsDefined(typeof(DefaultValueAttribute), false))
                {
                    var attrs = attributeProvider.GetCustomAttributes(typeof(DefaultValueAttribute), false);

                    foreach (object ax in attrs)
                    {
                        DefaultValueAttribute attr = ax as DefaultValueAttribute;
                        if (attr == null) continue;
//                        if (property.PropertyType.IsGenericType &&
//property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
//                        {
//                            //Type type = property.PropertyType.GetGenericArguments()[0];
//                            //var converter = new NullableConverter(property.PropertyType);
//                            //object converted = Convert.ChangeType(attr.Value, property.PropertyType);
//                            object converted = Activator.CreateInstance(property.PropertyType, attr.Value);

//                            //object converted = converter.ConvertFrom(attr.Value);
//                            property.DefaultValue = converted;
//                            //property.DefaultValue = converter(attr.Value, property.PropertyType);
//                        }
//                        else
                        {
                            property.DefaultValue = attr.Value;
                        }
                        break;
                    }
                }
            }
        }
    }
}
#endif