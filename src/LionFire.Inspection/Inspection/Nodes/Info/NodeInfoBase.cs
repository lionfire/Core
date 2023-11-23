using LionFire.IO;

namespace LionFire.Inspection.Nodes;

public class NodeInfo : NodeInfoBase, INodeInfo
{
    public NodeInfo(InspectorNodeKind nodeKind)
    {
        this.nodeKind = nodeKind;
    }

    public override InspectorNodeKind NodeKind => nodeKind;
    private InspectorNodeKind nodeKind = InspectorNodeKind.Unspecified;
}

public abstract class NodeInfoBase : INodeInfo
{
    public string? Name { get; init; }

    public string? OrderString { get; init; }
    public float? Order { get; init; }

    public Type? Type { get; init; }
    public virtual bool IsAsync => false;
    public abstract InspectorNodeKind NodeKind { get; }
    public IODirection IODirection { get; init; }

    public IEnumerable<string> Flags { get; init; } = Enumerable.Empty<string>();
}
