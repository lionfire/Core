using LionFire.Data.Gets;
using LionFire.Data.Sets;

namespace LionFire.Data;

public interface IStatelessAsyncValue<T> : IStatelessGets<T>, ISets<T>
{
}
