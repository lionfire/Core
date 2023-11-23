using LionFire.Data.Async.Gets;
using LionFire.Data.Async.Sets;

namespace LionFire.Data.Async;

public abstract class StatelessValueSlim<TValue> : GetterSlim<TValue>, IStatelessAsyncValue<TValue>
{
    public abstract Task<ISetResult<TValue>> Set(TValue? value, CancellationToken cancellationToken = default);
}
