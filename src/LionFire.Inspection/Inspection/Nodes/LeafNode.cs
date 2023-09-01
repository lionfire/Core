namespace LionFire.Inspection.Nodes;

/// <summary>
///  Like ObjectNode, but no deeper inspection allowed, probably because it's a primitive, or introspection is not desired.
/// No groups or children, just a summarizer/editor.  
/// </summary>
public class LeafNode : Node
{
    public LeafNode(INode? parent, object? source, InspectorContext? context = null) 
        : base(parent, source, context)
    {
    }

    public override INodeInfo Info => throw new NotImplementedException();
}
