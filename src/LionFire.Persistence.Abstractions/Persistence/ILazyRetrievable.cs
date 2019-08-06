using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public interface ILazyRetrievable
    {
        Task<bool> TryGetObject();
    }
}
