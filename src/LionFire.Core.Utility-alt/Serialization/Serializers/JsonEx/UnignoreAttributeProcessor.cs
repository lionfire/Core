//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using JsonExSerializer.MetaData.Attributes;
//using JsonExSerializer.MetaData;
//using System.Reflection;
//using JsonExSerializer;

//namespace LionFire.Serialization.JsonEx
//{
//    /// <summary>
//    /// Unignore a member that would otherwise be ignored (such as a property that lacks public get/set methods)
//    /// </summary>
//    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
//    public sealed class UnignoreAttribute : Attribute
//    {
//        public UnignoreAttribute()
//        {
//        }
//    }

//    public class UnignoreAttributeProcessor : AttributeProcessor
//    {
//        public override void Process(IMetaData metaData, ICustomAttributeProvider attributeProvider, IConfiguration config)
//        {
//            if (metaData is IPropertyData)
//            {
//                IPropertyData property = (IPropertyData)metaData;
//                if (attributeProvider.IsDefined(typeof(UnignoreAttribute), false))
//                    property.Ignored = false;
//            }
//        }
//    }

//}
