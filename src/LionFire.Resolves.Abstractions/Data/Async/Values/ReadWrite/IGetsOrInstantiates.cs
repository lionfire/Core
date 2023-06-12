
using LionFire.Data.Async.Gets;

namespace LionFire.Data.Async.Sets;

public interface IGetsOrInstantiates<TValue> : IGets<TValue>, IInstantiatesForSet<TValue>
{
    ITask<IGetResult<TValue>> GetOrInstantiateValue();
}
