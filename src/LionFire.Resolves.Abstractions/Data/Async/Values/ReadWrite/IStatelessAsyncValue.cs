using LionFire.Data.Async.Gets;
using LionFire.Data.Async.Sets;

namespace LionFire.Data.Async;

public interface IStatelessAsyncValue<T> : IGets<T>, ISets<T>
{
}
