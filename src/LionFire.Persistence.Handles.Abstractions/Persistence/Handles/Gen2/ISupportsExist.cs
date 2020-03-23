using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public interface ISupportsExist<T>
    {
        Task<bool> Exists();
    }
}
