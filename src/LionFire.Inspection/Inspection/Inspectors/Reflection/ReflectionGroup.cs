using ReactiveUI;


namespace LionFire.Inspection.Nodes;

public abstract class ReflectionGroup : InspectorGroup
{
    #region Lifecycle

    public ReflectionGroup(IInspector inspector, INode sourceNode, INode node) : base(inspector, sourceNode, node)
    {
        sourceNode.WhenAnyValue(n => n.Source)
            .Subscribe(newNode => SourceType = newNode?.GetType());
    }

    #endregion

    #region State

    [Reactive]
    public Type? SourceType { get; set; }

    #endregion

    #region Value

    //public override IEnumerable<INode>? Value => readCacheValue;
    //public override void DiscardValue() => readCacheValue = Enumerable.Empty<INode>();

    //public override IEnumerable<INode>? ReadCacheValue => readCacheValue;
    //protected IEnumerable<INode>? readCacheValue = Enumerable.Empty<INode>();


    #endregion

}
