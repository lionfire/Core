using LionFire.Data.Async.Sets;

namespace LionFire.Inspection.Nodes;

public abstract class SetterNode<TInfo> : InspectedNode<TInfo>
    where TInfo : INodeInfo
{
    protected SetterNode(TInfo info, INode? parent = null, object? source = null, string? key = null, InspectorContext? context = null) : base(info, parent, source, key, context)
    {
    }

    public abstract IStagesSet<object> Setter { get; }
    public override object? Value { get => Setter.StagedValue; set => Setter.StagedValue = value; }
}
