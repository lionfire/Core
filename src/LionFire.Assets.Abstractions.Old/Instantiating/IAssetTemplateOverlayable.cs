#if OLD // Use ITemplateOverlayable
using LionFire.Instantiating;
using System.Collections.Generic;

namespace LionFire.Assets
{
    public interface IAssetTemplateOverlayable
    {
        object OverlayParent { get; set; }
        ParameterOverlayMode OverlayMode { get; set; }
#if !AOT
        IEnumerable<IEnumerable<IAssetInstantiation>> OverlayTargets { get; }
#endif

    }
    
}
#endif