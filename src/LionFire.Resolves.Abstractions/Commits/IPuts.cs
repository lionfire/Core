using LionFire.Results;
using System.Threading.Tasks;

namespace LionFire.Resolves
{
    public interface IPuts
    {
        Task<ISuccessResult> Put();
    }
}
