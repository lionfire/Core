using LionFire.Data.Collections;

namespace LionFire.Inspection.Nodes;

public interface IInspectorGroup : INode
{
    IInspector Inspector { get; }
    GroupInfo Info { get; }

    IAsyncReadOnlyDictionary<string, INode> Children { get; }
}

