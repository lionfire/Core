using System.Collections.Generic;

namespace LionFire.Instantiating
{
    public interface ITemplateOverlayable
    {
        object OverlayParent { get; set; }
        ParameterOverlayMode OverlayMode { get; set; }
#if !AOT
        IEnumerable<IEnumerable<IInstantiation>> OverlayTargets { get; }
#endif

    }
}
