
using LionFire.Data.Gets;

namespace LionFire.Data.Sets;

public interface IGetsOrInstantiates<TValue> : IStatelessGets<TValue>, IInstantiatesForSet<TValue>
{
    ITask<IGetResult<TValue>> GetOrInstantiateValue();
}
