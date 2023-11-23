
namespace LionFire.FlexObjects;

public static class RecursiveToSingleFlexX
{

    #region RecursiveQuery 

    // Convenience: Missing key parameter
    public static TCriteria? RecursiveQuery<TNode, TCriteria>(this TNode o, Func<TNode, TNode?> getNext)
        where TNode : IFlex
        => o.RecursiveQuery<TNode, TCriteria>(null, getNext);

    public static TCriteria? RecursiveQuery<TNode, TCriteria>(this TNode node, string? key, Func<TNode, TNode?> getNext)
        where TNode : IFlex
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(node);
#endif

        TNode? next = node;
        do
        {
            if (next.Query<TCriteria>(out var result, key)) { return result; }
            next = getNext(next);
        } while (next != null);
        return default;
    }

    #endregion

    #region RecursiveQuery (non-generic)

    // Convenience: Missing key parameter
    public static object? RecursiveQuery(this IFlex node, Type type, Func<IFlex, IFlex?> getNext)
        => RecursiveQuery(node, type, null, getNext);
    public static object? RecursiveQuery(this IFlex node, Type type, string? key, Func<IFlex, IFlex?> getNext)
    {
        if (node == null || type == null) return null;

        IFlex? next = node;
        do
        {
            if (next.Query(type, out var result, key)) { return result; }
            next = getNext(next);
        } while (next != null);
        return default;
    }

    #endregion
}
