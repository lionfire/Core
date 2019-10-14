using LionFire.Instantiating;
using System.Collections.Generic;

namespace LionFire.Assets
{

    public interface IAssetInstantiation : IInstantiation, ITemplateOverlayable, IHasTemplateAsset
    // TODO:  move ITemplateOverlayable to IInstantiationBase
    {
        //#if !AOT
        //        IEnumerable<IAssetInstantiation> AllChildren { get; }
        //#endif
        //Func<Instantiation, string> GetDefaultKey { get; }
        //AssetInstantiationCollection Children { get; set; }
    }

#if !AOT
    public interface IAssetInstantiation<InstanceType> : IAssetInstantiation
        where InstanceType : ITemplateAssetInstance, new()
        //where TemplateType : ITemplate
    {
    }
#endif
}
