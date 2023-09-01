using System.Reactive.Linq;

namespace LionFire.Inspection.Nodes;

public abstract class HierarchicalNode<TInfo> : Node<TInfo>, IHierarchicalNode
    where TInfo : INodeInfo
{
    #region Lifecycle

    protected HierarchicalNode(INode? parent, object? source, InspectorContext? context = null)
        : base(parent, source, context)
    {
    }

    #endregion

    #region Groups

    public virtual SourceCache<InspectorGroup, string>? Groups => null;

    #endregion

    #region Children

    public virtual IObservable<bool?> HasChildren { get; } = Observable.Return((bool?)false);
    public virtual IObservableCache<INode, string>? Children => empty.AsObservableCache();
    private static readonly SourceCache<INode, string> empty = new SourceCache<INode, string>(n => n.Key);

    #endregion

    #region IKeyProvider

    public abstract string GetKey(INode? node);

    #endregion
}
