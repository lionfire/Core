
using LionFire.Data.Gets;

namespace LionFire.Data.Sets;

public interface IGetsOrAsyncInstantiates<TValue> : IStatelessGets<TValue>, IAsyncInstantiatesForSet<TValue>
{
    ITask<IGetResult<TValue>> GetOrAsyncInstantiateValue();
}
