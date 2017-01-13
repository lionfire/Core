using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating.Templating
{
    public class TemplateAssetInstantiationProvider : IInstantiationProvider
    {
        private ITemplateAsset ta;

        public TemplateAssetInstantiationProvider(ITemplateAsset ta)
        {
            this.ta = ta;
        }

        public IInstantiator TryProvide(object instance, InstantiationContext context = null)
        {
            if (ta == null) return null;

            return new TemplateAssetInstantiation
            {
                AssetSubPath = ta.AssetSubPath,
                TypeName = instance.GetType().FullName, // TODO ENH: alias system for short names
            };
                //context.Naming.GetName(instance.GetType())
        }
    }

    public class TemplateAssetInstantiation : AssetInstantiator
    {

    }
}
