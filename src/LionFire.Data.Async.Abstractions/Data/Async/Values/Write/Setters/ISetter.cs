using LionFire.Data.Async.Gets;
using LionFire.Persistence;
using LionFire.Results;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Sets;

public interface ISetter<in TKey, TValue>
{
    Task<ISetResult<TValue>> Set(TKey key, TValue value);
}

#if false
// ?
public interface ISetterContravariant<in TKey, in TValue>
{
    ITask<ISetResult<TValue>> Set(TKey key, TValue value);
}
#endif