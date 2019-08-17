using System.Threading.Tasks;

namespace LionFire.Persistence.Implementation
{
    public interface IRetrievableImpl
    {
        /// <summary>
        /// Force a retrieve of the reference from the source.  Replace the Object.
        /// </summary>
        /// <remarks>Can't return a generic IRetrieveResult due to limitation of the language.</remarks>
        Task<IPersistenceResult> TryRetrieveObject();
    }

    
}
