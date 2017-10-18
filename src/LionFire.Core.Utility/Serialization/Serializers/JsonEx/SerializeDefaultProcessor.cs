using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonExSerializer.MetaData.Attributes;
using JsonExSerializer.MetaData;
using System.Reflection;
using JsonExSerializer;

namespace LionFire.Serialization.JsonEx
{
    public class SerializeDefaultProcessor : AttributeProcessor
    {
        public override void Process(IMetaData metaData, ICustomAttributeProvider attributeProvider, IConfiguration config)
        {
            if (metaData is IPropertyData)
            {
                IPropertyData property = (IPropertyData)metaData;
                if (attributeProvider.IsDefined(typeof(SerializeDefaultValueAttribute), false))
                {
                    var attrs = attributeProvider.GetCustomAttributes(typeof(SerializeDefaultValueAttribute), false);

                    foreach (object ax in attrs)
                    {
                        SerializeDefaultValueAttribute attr = ax as SerializeDefaultValueAttribute;
                        if (attr == null) continue;
                        if(attr.SerializeDefaultValue.HasValue)
                        {
                            property.DefaultValueSetting = attr.SerializeDefaultValue.Value ? DefaultValueOption.WriteAllValues : DefaultValueOption.SuppressDefaultValues;
                            break;
                        }
                    }                    
                }
            }
        }
    }


}
