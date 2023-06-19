using System.Threading.Tasks;

namespace LionFire.Persistence.Implementation
{
    public interface ICommitableImpl
    {
        /// <summary>
        /// Submit pending changes to the underlying store
        /// </summary>
        Task<ITransferResult> Commit();
    }
}
