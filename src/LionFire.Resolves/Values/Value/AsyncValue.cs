using LionFire.Data.Async.Gets;
using LionFire.Results;

namespace LionFire.Data.Async;

public abstract class AsyncValue<TValue> : Gets<TValue>, IAsyncValue<TValue>
{
    public abstract Task<ISuccessResult> Set(TValue? value, CancellationToken cancellationToken = default);
}

// See also: AsyncValueRxEx
public abstract class AsyncValueEx<TValue> : AsyncValue<TValue>, IAsyncValueEx<TValue>
{
    public abstract TValue? StagedValue { get; set; }
    public abstract bool HasStagedValue { get; set; }

    public abstract void DiscardStagedValue();
    public abstract ITask<IGetResult<TValue>> GetOrAsyncInstantiateValue();
    public abstract ITask<TValue> InstantiateValue(bool overwriteStagedValue = false, bool throwOnOverwrite = false);
    public abstract Task<ISuccessResult> Set(CancellationToken cancellationToken = default);
}