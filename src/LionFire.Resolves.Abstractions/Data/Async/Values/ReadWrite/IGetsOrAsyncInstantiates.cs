
using LionFire.Data.Async.Gets;

namespace LionFire.Data.Async.Sets;

public interface IGetsOrAsyncInstantiates<TValue> : IGets<TValue>, IAsyncInstantiatesForSet<TValue>
{
    ITask<IGetResult<TValue>> GetOrAsyncInstantiateValue();
}
