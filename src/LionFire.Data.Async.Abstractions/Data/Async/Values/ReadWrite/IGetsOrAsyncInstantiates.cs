
using LionFire.Data.Gets;

namespace LionFire.Data.Sets;

public interface IGetsOrAsyncInstantiates<TValue> : IGets<TValue>, IAsyncInstantiatesForSet<TValue>
{
    ITask<IGetResult<TValue>> GetOrAsyncInstantiateValue();
}
