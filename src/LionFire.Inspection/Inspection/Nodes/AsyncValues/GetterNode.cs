using LionFire.Data.Async.Gets;

namespace LionFire.Inspection.Nodes;

public abstract class GetterNode<TInfo> : InspectedNode<TInfo>
    where TInfo : INodeInfo
{
    protected GetterNode(TInfo info, INode? parent = null, object? source = null, string? key = null, InspectorContext? context = null) : base(info, parent, source, key, context)
    {
    }

    public abstract IGetter<object> Getter { get; }
    public override object? Value { get => Getter.Value; set => throw new NotSupportedException(); }
}
