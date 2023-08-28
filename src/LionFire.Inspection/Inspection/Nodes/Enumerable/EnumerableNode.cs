
using DynamicData.Binding;

namespace LionFire.Inspection.Nodes;

public class EnumerableNode : Node, INode
{
    public EnumerableNode(INode? parent, object source) : base(parent, source)
    {
    }

    // Children: the child items
    // Groups:
    //   - the primary Collection group
    public override SourceCache<InspectorGroup, string> Groups => throw new NotImplementedException();


    public override INodeInfo Info => info;
    private static readonly NodeInfo info = new()
    {
        Name = "Enumerable",
        Order = "-1",
        Flags = Enumerable.Empty<string>(),
        NodeKind = InspectorNodeKind.Enumerable,
    };
}
