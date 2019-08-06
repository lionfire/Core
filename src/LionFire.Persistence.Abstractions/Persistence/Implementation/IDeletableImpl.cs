using System.Threading.Tasks;

namespace LionFire.Persistence.Implementation
{
    public interface IDeletableImpl
    {
        Task<IPersistenceResult> Delete();
    }
}
