using LionFire.Resolves;
using System.Threading.Tasks;

namespace LionFire.Resolvers
{
    public interface ICommitter<in TKey, in TValue>
    {
        Task<IPutResult> Commit(TKey key, TValue value);
    }
}
