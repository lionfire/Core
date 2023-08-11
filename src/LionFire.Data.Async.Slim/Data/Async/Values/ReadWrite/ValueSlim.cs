using LionFire.Data.Async.Sets;

namespace LionFire.Data.Async;

// See also: AsyncValueRxEx
public abstract class ValueSlim<TValue> : StatelessValueSlim<TValue>, IValue<TValue>
{
    public abstract TValue? StagedValue { get; set; }
    public abstract bool HasStagedValue { get; set; }

    public abstract void DiscardStagedValue();
    public abstract ITask<IGetResult<TValue>> GetOrAsyncInstantiateValue();
    public abstract ITask<TValue> InstantiateValue(bool overwriteStagedValue = false, bool throwOnOverwrite = false);
    public abstract Task<ISetResult> Set(CancellationToken cancellationToken = default);
}