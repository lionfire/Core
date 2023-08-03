using LionFire.Data.Async.Gets;
using LionFire.Data.Async.Sets;

namespace LionFire.Data.Async;

public interface IStatelessAsyncValue<T> : IStatelessGetter<T>, ISetter<T>
{
}
