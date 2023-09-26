using LionFire.Data.Async;
using ReactiveUI;

namespace LionFire.Inspection.Nodes;

public abstract class AsyncValueNode<TInfo> : InspectedNode<TInfo>
    where TInfo : INodeInfo
{
    protected AsyncValueNode(TInfo info, INode? parent = null, object? source = null, string? key = null, InspectorContext? context = null) : base(info, parent, source, key, context)
    {
    }

    public abstract IAsyncValue<object?> AsyncValue { get; }
    public override object? Value { get => AsyncValue.Value; set => AsyncValue.Value = value; }
    public override IObservable<object?> Values => AsyncValue.WhenAnyValue(x => x.Value);
}
