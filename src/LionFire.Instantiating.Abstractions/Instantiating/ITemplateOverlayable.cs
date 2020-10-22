using System;
using System.Collections.Generic;
using System.Linq;

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

    public static class ITemplateOverlayableExtensions
    {
        public static bool TryGetOverlayParent(this IInstantiation i)
        {
            if (i.OverlayParent != null) return true;
            if (i.OverlayMode == ParameterOverlayMode.Children)
            {
                if (i.Key == null) return true;

                var top = i.TopParent() as ITemplateOverlayable;
                if (top == null)
                {
                    top = i;
                    throw new Exception("TopParent is not a ITemplateOverlayable"); // TODO - this was a warning.  Is it ok if top isn't overlayable?
                }

                foreach (var overlayTarget in top.OverlayTargets)
                {
                    object overlayParent = overlayTarget.SelectRecursive<IInstantiation>().Where(pm => pm.Key == i.Key) // FIXME - Review SelectRecursive implementation
#if DEBUG
                        .SingleOrDefault();
#else
                        .FirstOrDefault();
#endif

                    if (overlayParent != null)
                    {
                        i.OverlayParent = overlayParent;
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
    }
}
