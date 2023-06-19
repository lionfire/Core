using LionFire.Data.Async.Gets;
using LionFire.Persistence;
using LionFire.Results;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Sets;

public interface ISetter<in TKey, in TValue>
{
    Task<ITransferResult> Set(TKey key, TValue value);
}
