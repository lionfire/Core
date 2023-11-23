using LionFire.IO;

namespace LionFire.Inspection.Nodes;

public class TransformInfo : INodeInfo
{
    public Type Type { get; set; }

    public string? Name => throw new NotImplementedException();

    public string? OrderString => throw new NotImplementedException();

    public InspectorNodeKind NodeKind => InspectorNodeKind.Transform;

    public IEnumerable<string> Flags => throw new NotImplementedException();

    public IODirection IODirection => throw new NotImplementedException();

    public float? Order => throw new NotImplementedException();
}
