using LionFire.Instantiating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Assets
{

    public interface IAssetInstantiation
        : IInstantiationBase, IAssetTemplateOverlayable, IHasTemplateAsset
    {
#if !AOT
        IEnumerable<IAssetInstantiation> AllChildren { get; }
#endif
        //Func<Instantiation, string> GetDefaultKey { get; }
        AssetInstantiationCollection Children { get; set; }
    }

#if !AOT
    public interface IAssetInstantiation<InstanceType> : IAssetInstantiation
        where InstanceType : ITemplateAssetInstance, new()
        //where TemplateType : ITemplate
    {
    }
#endif
}
