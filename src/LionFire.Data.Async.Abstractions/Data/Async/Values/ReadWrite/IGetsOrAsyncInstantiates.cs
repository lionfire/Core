
using LionFire.Data.Async.Gets;

namespace LionFire.Data.Async.Sets;

public interface IGetsOrAsyncInstantiates<TValue> : IStatelessGetter<TValue>, IAsyncInstantiatesForSet<TValue>
{
    ITask<IGetResult<TValue>> GetOrAsyncInstantiateValue();
}
