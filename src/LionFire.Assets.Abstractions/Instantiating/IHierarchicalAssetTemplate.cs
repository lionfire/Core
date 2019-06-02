using LionFire.Instantiating;
using System.Collections.Generic;

namespace LionFire.Assets
{
    public interface IHierarchicalAssetTemplate : ITemplate
    {
        List<ITemplateAsset> Children { get; set; }
    }
}
