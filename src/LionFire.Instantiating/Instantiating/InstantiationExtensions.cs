using System.Collections.Generic;
using System.Linq;

namespace LionFire.Instantiating
{
    public static class InstantiationExtensions
    {
        public static Instantiation CreateInstantiation(this ITemplate template) => new Instantiation(template);

        public static IEnumerable<IInstantiation> AllChildren(this Instantiation instantiation)
        {
            if (instantiation.HasChildren)
            {
#if AOT
					foreach (LionFire.IEnumerableExtensions.Recursion child in (this.Children.SelectRecursive(x => ((IAssetInstantiation)x).GetChildrenEnumerable())))
					{
						yield return child.Item;
					}
#else
                foreach (var child in instantiation.Children.SelectRecursive(x => x.GetChildrenEnumerable()).Select(r => r.Item))
                {
                    yield return child;
                }
#endif
            }
        }
    }
}
