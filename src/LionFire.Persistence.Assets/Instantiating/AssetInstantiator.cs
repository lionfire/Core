using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Assets;
using LionFire.Types;

namespace LionFire.Instantiating
{
    public class AssetInstantiator : IInstantiator
        //, IAsset
    {
        public string TypeName { get; set; }
        public string AssetSubPath { get; set; }

        public virtual object Affect(object obj, InstantiationContext context = null)
        {
            var type = TypeResolver.Default.Resolve(TypeName);
            if (type == null) throw new TypeNotFoundException();
            return AssetSubPath.Load(TypeName, context);
        }
    }
}
