using LionFire.Data.Async.Gets;

namespace LionFire.Data.Async;

public abstract class StatelessValueSlim<TValue> : GetterSlim<TValue>, IStatelessAsyncValue<TValue>
{
    public abstract Task<ITransferResult> Set(TValue? value, CancellationToken cancellationToken = default);
}
