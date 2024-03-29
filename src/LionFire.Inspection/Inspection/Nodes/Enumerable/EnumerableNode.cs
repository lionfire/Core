﻿
using DynamicData.Binding;

namespace LionFire.Inspection.Nodes;

public class EnumerableNode : Node<NodeInfoBase>, INode
{
    public EnumerableNode(INode? parent, object source) : base(parent, source, info)
    {
    }

    // Children: the child items
    // Groups:
    //   - the primary Collection group
    //public override SourceCache<IInspectorGroup, string> Groups => base.Groups throw new NotImplementedException();


    //public override INodeInfo Info => info;
    private static readonly NodeInfo info = new(InspectorNodeKind.Enumerable)
    {
        Name = "Enumerable",
        OrderString = "-1",
        Flags = Enumerable.Empty<string>(),
    };
}
