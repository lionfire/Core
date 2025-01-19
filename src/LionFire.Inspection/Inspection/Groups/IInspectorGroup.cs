using LionFire.Data.Collections;

namespace LionFire.Inspection.Nodes;

public interface IInspectorGroup : INode
{
    IInspector Inspector { get; }
    GroupInfo Info { get; }

    IAsyncReadOnlyKeyedCollection<string, INode> Children { get; }
}

