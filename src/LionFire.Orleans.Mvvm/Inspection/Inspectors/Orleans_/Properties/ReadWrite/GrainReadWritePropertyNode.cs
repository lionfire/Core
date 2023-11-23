using LionFire.Inspection.Nodes;
using LionFire.Data.Async;
using LionFire.Data.Async.Gets;
using LionFire.Structures;
using LionFire.Data.Async.Sets;
using LionFire.Threading;
using System.Diagnostics;

namespace LionFire.Inspection;

public class GrainReadWritePropertyNode : AsyncValueNode<GrainPropertyInfo>
    //, InspectedNode // OLD
    //, IAsyncValue<object> // TEMP
{
    
    public GrainReadWritePropertyNode(INode? parent, object? source, GrainPropertyInfo info, string? key = null, InspectorContext? context = null) : base(parent: parent, source: source, info: info, key: key, context: context)
    {
        Key = info.Name ?? "(null)";
        Values.Subscribe(v => // TEMP
        {
            Debug.WriteLine($"GrainReadWritePropertyNode new value: {v}");
        });
        AsyncValue.GetOperations.Subscribe(async t =>
        {
            var r = await t.ConfigureAwait(false);
            Debug.WriteLine($"GrainReadWritePropertyNode GetOperations.OnNext: {r.ToDebugString()}");
        });
        AsyncValue.Get().AsTask().FireAndForget(); // TEMP
    }

    public override IAsyncValue<object?> AsyncValue => asyncValue ??= new GrainPropertyAsyncValue(this);
    private GrainPropertyAsyncValue? asyncValue;

    #region TEMP: Pass-thru

    // TEMP

    //public ValueOptions Options { get => ((IValueRxO<object>)asyncValue).Options; set => ((IValueRxO<object>)asyncValue).Options = value; }

    //public object? ReadCacheValue => ((IGetter<object>)asyncValue).ReadCacheValue;



    //public bool HasValue => ((IDefaultable)asyncValue).HasValue;

    //public IObservable<ITask<IGetResult<object>>> GetOperations => ((IObservableGetOperations<object>)asyncValue).GetOperations;

    //public IObservable<ISetOperation<object>> SetOperations => ((IObservableSetOperations<object>)asyncValue).SetOperations;

    //public IObservable<ISetResult<object>> SetResults => ((IObservableSetResults<object>)asyncValue).SetResults;

    //public object? StagedValue { get => ((IStagesSet<object>)asyncValue).StagedValue; set => ((IStagesSet<object>)asyncValue).StagedValue = value; }
    //public bool HasStagedValue { get => ((IWriteStagesSet<object>)asyncValue).HasStagedValue; set => ((IWriteStagesSet<object>)asyncValue).HasStagedValue = value; }
    //public override object Value { get => ((IAsyncValue<object>)asyncValue).Value; set => ((IAsyncValue<object>)asyncValue).Value = value; }

    //object? IReadWrapper<object>.Value => ((IReadWrapper<object>)asyncValue).Value;

    //public void Discard() => ((IDiscardable)asyncValue).Discard();

    //public void DiscardStagedValue() => ((IWriteStagesSet<object>)asyncValue).DiscardStagedValue();

    //public void DiscardValue() => ((IDiscardableValue)asyncValue).DiscardValue();

    //public ITask<IGetResult<object>> Get(CancellationToken cancellationToken = default) => ((IStatelessGetter<object>)asyncValue).Get(cancellationToken);

    //public ITask<IGetResult<object>> GetIfNeeded() => ((IGetter<object>)asyncValue).GetIfNeeded();

    //public IGetResult<object> QueryValue() => ((IGetter<object>)asyncValue).QueryValue();

    //public Task<ISetResult> Set(CancellationToken cancellationToken = default) => ((ISetter)asyncValue).Set(cancellationToken);

    //public Task<ISetResult<object>> Set(object? value, CancellationToken cancellationToken = default) => ((ISetter<object>)asyncValue).Set(value, cancellationToken);

    #endregion

}
