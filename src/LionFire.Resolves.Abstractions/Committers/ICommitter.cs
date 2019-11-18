using LionFire.Resolves;
using LionFire.Results;
using System.Threading.Tasks;

namespace LionFire.Resolvers
{
    public interface ICommitter<in TKey, in TValue>
    {
        Task<ISuccessResult> Commit(TKey key, TValue value);
    }
}
