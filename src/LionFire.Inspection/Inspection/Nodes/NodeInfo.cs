using LionFire.IO;

namespace LionFire.Inspection.Nodes;

public class NodeInfo : INodeInfo
{
    public string? Name { get; init; }

    public string? Order { get; init; }

    public Type? Type { get; init; }
    public InspectorNodeKind NodeKind { get; init; }
    public IODirection IODirection { get; init; }

    public IEnumerable<string> Flags { get; init; } = Enumerable.Empty<string>();
}
