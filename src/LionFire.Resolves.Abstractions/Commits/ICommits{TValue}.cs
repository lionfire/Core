using System.Threading.Tasks;

namespace LionFire.Resolves
{
    public interface IPuts<in TValue>
    {
        Task<IPutResult> Put(TValue value);
    }
}
