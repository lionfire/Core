using LionFire.Ontology;

namespace LionFire.FlexObjects;

public static class ParentedRecursiveFlexX
{
    public static TCriteria? RecursiveQuery<TNode, TCriteria>(this TNode o, string? key = null)
        where TNode : IFlex, IParented<TNode>
        => o.RecursiveQuery<TNode, TCriteria>(key, o => o.Parent);


    public static TCriteria? RecursiveQuery<TCriteria>(this IFlex node, string? key = null) 
        => node.RecursiveQuery<IFlex, TCriteria>(key, f => f is IParented<IFlex> p ? p.Parent : null);

    public static object? RecursiveQuery(this IFlex node, Type type, string? key = null)
        => node.RecursiveQuery(type, key, f => f is IParented<IFlex> p ? p.Parent : null);
}

