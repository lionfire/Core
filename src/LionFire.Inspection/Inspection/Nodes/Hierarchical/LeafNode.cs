namespace LionFire.Inspection.Nodes;

/// <summary>
///  Like ObjectNode, but no deeper inspection allowed, probably because it's a primitive, or introspection is not desired.
/// No groups or children, just a summarizer/editor.  
/// </summary>
public class LeafNode<TNodeInfo> : Node<TNodeInfo>
    where TNodeInfo : INodeInfo
{
    public LeafNode(INode? parent, object? source, TNodeInfo info, string? key = null, InspectorContext? context = null)
        : base(parent, source, info, key, context: context)
    {
    }
}
