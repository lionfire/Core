using LionFire.Inspection.Nodes;
using LionFire.Data.Async.Gets;
using LionFire.Structures;
using LionFire.Threading;

namespace LionFire.Inspection;

public class GrainReadPropertyNode : Node<GrainPropertyInfo>, IGetter<object>
{
    public GrainReadPropertyNode(INode? parent, object? source, GrainPropertyInfo info, string? key = null, InspectorContext? context = null) : base(parent, source, info, key, context)
    {
        Getter = new GrainPropertyGetter(this);
        Getter.Get().AsTask().FireAndForget(); // TEMP
    }

    GrainPropertyGetter Getter { get; set; }

    #region Pass-thru

    public object? ReadCacheValue => ((IGetter<object>)Getter).ReadCacheValue;

    public object? Value => ((IReadWrapper<object>)Getter).Value;

    public bool HasValue => ((IDefaultable)Getter).HasValue;

    public IObservable<ITask<IGetResult<object>>> GetOperations => ((IObservableGetOperations<object>)Getter).GetOperations;


    public void Discard()
    {
        ((IDiscardable)Getter).Discard();
    }

    public void DiscardValue()
    {
        ((IDiscardableValue)Getter).DiscardValue();
    }

    public ITask<IGetResult<object>> Get(CancellationToken cancellationToken = default)
    {
        return ((IStatelessGetter<object>)Getter).Get(cancellationToken);
    }

    public ITask<IGetResult<object>> GetIfNeeded()
    {
        return ((IGetter<object>)Getter).GetIfNeeded();
    }

    public IGetResult<object> QueryGetResult()
    {
        return ((IGetter<object>)Getter).QueryGetResult();
    }

    #endregion

}
