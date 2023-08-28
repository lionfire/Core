﻿
using DynamicData.Binding;

namespace LionFire.Inspection.Nodes;

public class ObjectNode : Node, INode
{
    // Children: a flattened collection of the INodes that have been retrieved from all of the Groups
    // Groups: pluggable

    public ObjectNode(object source, InspectorContext context) : base(null, source, context)
    {
        Info = ObjectNodeInfo.Default;
    }

    public ObjectNode(INode? parent, object source, INodeInfo info, InspectorContext? inspectorContext = null) : base(parent, source, inspectorContext)
    {
        Info = info;
    }

    public override INodeInfo Info { get; }




    public override SourceCache<InspectorGroup, string> Groups => throw new NotImplementedException();
}

public class ObjectNodeInfo : NodeInfo
{
    public static ObjectNodeInfo Default { get; } = new ObjectNodeInfo();

    #region Lifecycle

    public ObjectNodeInfo()
    {
        Name = "(custom object)";
        NodeKind = InspectorNodeKind.Object;
    }

    #endregion
}