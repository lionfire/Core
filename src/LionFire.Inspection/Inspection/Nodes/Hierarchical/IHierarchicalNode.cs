using LionFire.Data.Collections;
using System.Reactive.Linq;

namespace LionFire.Inspection.Nodes;

/// <remarks>
/// Inheritors:
///  - IInspectedNode: Children consist of GroupNodes.  NodeVM might flatten Groups' Children into a single collection.
///  - IGroupNode: Children are directly provided by the particular GroupNode implementation
/// </remarks>
public interface IHierarchicalNode
    : INode
    //, IKeyProvider<string, INode> // TODO: Eliminate this? Bring it back on individual INode implementations if needed?
{
    IAsyncReadOnlyDictionary<string, INode> Children { get; }

    IObservable<bool?> HasChildren => Children.ObservableCache.CountChanged.Select(c => (bool?)(c > 0));
}

