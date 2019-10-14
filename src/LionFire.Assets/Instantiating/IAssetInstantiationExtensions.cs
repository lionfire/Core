using LionFire.Instantiating;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.Assets
{
    public static class IAssetInstantiationExtensions
    {
#if TOPORT
        public static IEnumerable
#if !AOT
            <IAssetInstantiation>
#endif
                GetChildrenEnumerableRecursive(this IAssetInstantiation instantiation)
        {
            if (instantiation == null) yield break;
            if (!instantiation.HasChildren) yield break;

#if AOT
			foreach (LionFire.IEnumerableExtensions.Recursion child in (instantiation.Children.SelectRecursive(x => ((IAssetInstantiation)x))))
#else
            foreach (var child in (instantiation.Children.SelectRecursive(x => x.Children).Select(y => y.Item)))
#endif
            {

                yield return child
#if AOT
					.Item
#endif
                    ;
            }
        }

#if !AOT
        public static bool TryGetOverlayParent(this IAssetInstantiation i)
        {
            if (i.OverlayParent != null) return true;
            if (i.OverlayMode == ParameterOverlayMode.Children)
            {
                if (i.Key == null) return true;

                var top = i.TopParent() as ITemplateOverlayable;
                if (top == null)
                {
                    top = i;
                    l.Warn("TopParent is not a ITemplateOverlayable");
                }

                foreach (var overlayTarget in top.OverlayTargets)
                {
                    object overlayParent = overlayTarget.SelectRecursive<IAssetInstantiation>().Where(pm => pm.Key == i.Key)
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
#endif

#endif

        // TOPORT if needed
        //public static IEnumerable<IAssetInstantiation> GetChildrenEnumerable(this IAssetInstantiation instantiation)
        //{
        //    if (instantiation == null) yield break;
        //    if (!instantiation.HasChildren) yield break;

        //    foreach (var child in instantiation.AllChildren())
        //    {
        //        yield return child;
        //    }
        //}

        #region Misc

        private static ILogger l = Log.Get();

        #endregion

    }
}