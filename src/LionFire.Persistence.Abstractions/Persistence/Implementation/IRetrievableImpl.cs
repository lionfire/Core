#if UNUSED
using System.Threading.Tasks;

namespace LionFire.Persistence;

public interface IRetrievableImpl<TObject>
{
    /// <summary>
    /// Force a retrieve of the reference from the source.  Replace the Object.
    /// </summary>
    /// <remarks>Can't return a generic IGetResult due to limitation of the language.</remarks>
    /// <returns>true if an object was retrieved.  False if object was not found at location of the Reference.  Throws if could not resolve the Reference to a valid source.</returns>
    Task<IGetResult<TObject>> RetrieveImpl();
}

#endif