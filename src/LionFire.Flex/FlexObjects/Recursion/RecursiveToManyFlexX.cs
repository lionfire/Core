
namespace LionFire.FlexObjects;

public static class RecursiveToManyFlexX
{

    #region RecursiveFindFirst

    public static bool RecursiveFindFirstOfMany<TNode, TCriteria>(this TNode node, string key, Func<TNode, IEnumerable<TNode>> getNext, out TCriteria? result)
        where TNode : IFlex
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(node);
#endif
        if (node.Query<TCriteria>(out result, key)) { return true; }

        foreach (var x in getNext(node))
        {
            if (x.RecursiveFindFirstOfMany<TNode, TCriteria>(key, getNext, out result)) return true;
        }
        return false;
    }

    #endregion

    #region RecursiveFindAll

    // Convenience: Missing key parameter
    public static IEnumerable<TCriteria> RecursiveFindAll<TNode, TCriteria>(this TNode node, Func<TNode, IEnumerable<TNode>> getNext)
        where TNode : IFlex
        => node.RecursiveFindAll<TNode, TCriteria>(null, getNext);
    public static IEnumerable<TCriteria> RecursiveFindAll<TNode, TCriteria>(this TNode node, string? key, Func<TNode, IEnumerable<TNode>> getNext)
        where TNode : IFlex
    {
        var list = new List<TCriteria>();
        inner(node, key, getNext, list);
        return list;

        #region (local)

        static void inner<TNode2, TCriteria2>(TNode2 node, string? key, Func<TNode2, IEnumerable<TNode2>> getNext, List<TCriteria2> runningList)
            where TNode2 : IFlex
        {
            if (node.Query<TCriteria2>(out var result, key) && result != null) { runningList.Add(result); }

            foreach (var x in getNext(node))
            {
                inner(x, key, getNext, runningList);
            }
        }

        #endregion
    }

    #endregion
}
