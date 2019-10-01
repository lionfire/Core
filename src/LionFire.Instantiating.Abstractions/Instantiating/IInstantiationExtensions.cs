using System.Collections.Generic;

namespace LionFire.Instantiating
{
    public static class IInstantiationExtensions
    {
        // TODO - get from IAssetInstantiationExtensions?  Where did it go? Perhaps not needed?
        public static IEnumerable<IInstantiation> GetChildrenEnumerable(this IInstantiation instantiation)
        {
            if (instantiation == null) yield break;
            if (!instantiation.HasChildren) yield break;

            foreach (var child in instantiation.Children)
            {
                yield return child;
            }
        }
    }
}
