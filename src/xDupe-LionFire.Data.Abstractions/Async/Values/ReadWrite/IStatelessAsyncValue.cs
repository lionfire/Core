using LionFire.Data.Gets;
using LionFire.Data.Sets;

namespace LionFire.Data;

public interface IStatelessAsyncValue<T> : IGets<T>, ISets<T>
{
}
