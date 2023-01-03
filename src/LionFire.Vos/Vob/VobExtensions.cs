using LionFire.FlexObjects;
using LionFire.Referencing;
using LionFire.Vos.Internals;
using LionFire.Vos.Mounts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.Vos;

public static class VobExtensions
{
    /// <summary>
    /// Returns true if no information would be lost if the IVob was forgotten about.
    ///  - No Value
    ///  - Nothing in MultiType
    ///  - No VobNodes
    /// </summary>
    /// <param name="vob"></param>
    /// <returns></returns>
    public static bool IsEmpty(this IVob vob)
    {
        if (((IFlex)vob).IsEmpty()) return false;
        if (!vob.GetMultiTyped().IsEmpty) return false;
        if (((IVobInternals)vob).VobNodesByType.Any()) return false;
        return true;
    }

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
        => vob.Acquire<T>() ?? vob.Root.GetOrAddOwn<T>(valueFactory);

    public static HashSet<IMount> AllMountsRecursive(this IVob vob, HashSet<IMount> list = null)
    {
        list ??= new();

        var mounts = vob.Mounts()?.AllMounts;
        if (mounts != null)
            foreach (var mount in mounts)
            {
                list.Add(mount);
            }

        foreach (var child in vob.Children)
        {
            child.Value.AllMountsRecursive(list);
        }

        return list;
    }
}