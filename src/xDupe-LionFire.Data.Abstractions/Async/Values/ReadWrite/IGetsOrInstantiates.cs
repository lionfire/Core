
using LionFire.Data.Gets;

namespace LionFire.Data.Sets;

public interface IGetsOrInstantiates<TValue> : IGets<TValue>, IInstantiatesForSet<TValue>
{
    ITask<IGetResult<TValue>> GetOrInstantiateValue();
}
