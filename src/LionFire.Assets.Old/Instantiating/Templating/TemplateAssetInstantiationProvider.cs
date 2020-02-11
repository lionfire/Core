//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace LionFire.Instantiating.Templating
//{
//    public class TemplateAssetInstantiationProvider : IInstantiationProvider
//    {
//        private ITemplateAsset ta;

//        public TemplateAssetInstantiationProvider(ITemplateAsset ta)
//        {
//            this.ta = ta;
//        }

//        public IInstantiator TryProvide(object instance, InstantiationContext context = null)
//        {
//            if (ta == null) return null;

//            return new TemplateAssetInstantiation(instance.GetType())
//            {
//                AssetSubPath = ta.AssetSubPath,
//            };
//            //context.Naming.GetName(instance.GetType())
//        }
//    }

//    public class TemplateAssetInstantiation : AssetInstantiator
//    {
//        public TemplateAssetInstantiation() { }
//        public TemplateAssetInstantiation(Type type)
//        {
//            TypeName = type.FullName, // TODO ENH: alias system for short names
//        }

//    }
//}
