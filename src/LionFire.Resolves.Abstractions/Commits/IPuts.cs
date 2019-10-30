using System.Threading.Tasks;

namespace LionFire.Resolves
{
    public interface IPuts
    {
        Task<IPutResult> Put();
    }
}
