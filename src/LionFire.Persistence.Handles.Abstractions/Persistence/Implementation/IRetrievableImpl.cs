#if UNUSED
namespace LionFire.Persistence.Implementation;

public interface IRetrievableImpl
{
    /// <summary>
    /// Force a retrieve of the reference from the source.  Replace the Object.
    /// </summary>
    /// <remarks>Can't return a generic IGetResult due to limitation of the language.</remarks>
    Task<ITransferResult> TryRetrieveObject();
}
#endif