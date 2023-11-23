using ReactiveUI;


namespace LionFire.Inspection.Nodes;

public abstract class ReflectionGroup : FrozenGroup
{
    #region Lifecycle

    public ReflectionGroup(IInspector inspector, INode parent, GroupInfo info, string? key = null, InspectorContext? context = null) : base(inspector, parent, info, key, context)
    {
        //sourceNode.WhenAnyValue(n => n.Source)
            //.Subscribe(newNode => SourceType = newNode?.GetType());
    }

    #endregion

}
