#if TOMIGRATE // Newtonsoft Json
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
    public class IgnoreProcessor : AttributeProcessor
    {
        public LionSerializeContext IgnoreContexts = LionSerializeContext.All;

        public override void Process(IMetaData metaData, ICustomAttributeProvider attributeProvider, IConfiguration config)
        {
            if (metaData is IPropertyData)
            {
                IPropertyData property = (IPropertyData)metaData;
                if (attributeProvider.IsDefined(typeof(IgnoreAttribute), false))
                {
                    var attrs = attributeProvider.GetCustomAttributes(typeof(IgnoreAttribute), false);

                    foreach (object ax in attrs)
                    {
                        IgnoreAttribute attr = ax as IgnoreAttribute;
                        if (attr == null) continue;
                        if ((attr.Ignore & IgnoreContexts) != LionSerializeContext.None)
                        {                            
                            property.Ignored = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}
#endif
