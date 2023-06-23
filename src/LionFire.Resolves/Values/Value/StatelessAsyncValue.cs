using LionFire.Data.Async.Gets;
using LionFire.Results;

namespace LionFire.Data.Async;

public abstract class StatelessAsyncValue<TValue> : LazilyGets<TValue>, IStatelessAsyncValue<TValue>
{
    public abstract Task<ITransferResult> Set(TValue? value, CancellationToken cancellationToken = default);
}

// See also: AsyncValueRxEx
public abstract class AsyncValue<TValue> : StatelessAsyncValue<TValue>, IAsyncValue<TValue>
{
    public abstract TValue? StagedValue { get; set; }
    public abstract bool HasStagedValue { get; set; }

    public abstract void DiscardStagedValue();
    public abstract ITask<IGetResult<TValue>> GetOrAsyncInstantiateValue();
    public abstract ITask<TValue> InstantiateValue(bool overwriteStagedValue = false, bool throwOnOverwrite = false);
    public abstract Task<ITransferResult> Set(CancellationToken cancellationToken = default);
}