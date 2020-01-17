using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.Vos
{
    public static class VobExtensions
    {
        public static bool IsAncestorOf(this Vob potentialAncestor, Vob potentialChild)
        {
            for (IVob vobParent = potentialChild.Parent; vobParent != null; vobParent = vobParent.Parent)
            {
                if (ReferenceEquals(vobParent, potentialAncestor))
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetSubPathOfAncestor(this Vob vob, Vob potentialAncestor)
        {
            var result = vob.GetSubPathChunksOfAncestor(potentialAncestor);
            if (result == null)
            {
                return null;
            }

            return result.Aggregate((x, y) => x + LionPath.Separator + y);
        }

        public static IEnumerable<string> GetSubPathChunksOfAncestor(this Vob vob, Vob potentialAncestor)
        {
            var subPathChunks = new List<string>();

            for (var parent = vob.Parent; parent.Parent != parent; parent = parent.Parent)
            {
                if (parent == potentialAncestor)
                {
                    return ((IEnumerable<string>)subPathChunks).Reverse();
                }
            }
            return null;
        }

        public static T GetNextOrCreateAtRoot<T>(this IVob vob, Func<IVobNode, T> valueFactory = null, bool skipOwn = false) where T : class
            => vob.GetNext<T>() ?? vob.Root.GetOrAddOwn<T>(valueFactory);

    }
}