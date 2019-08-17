using System.Threading.Tasks;

namespace LionFire.Persistence
{
    /// <summary>
    /// An interface for directly initiating read-related persistence operations of single objects:
    ///  - Retrieve
    ///  - Exists
    /// </summary>
    public interface IRetrievable
    {
        /// <summary>
        /// Force a retrieve of the reference from the source.  Replace the Object.
        /// </summary>
        /// <remarks>Can't return a generic IRetrieveResult due to limitation of the language.</remarks>
        /// <returns>true if an object was retrieved.  False if object was not found at location of the Reference.  Throws if could not resolve the Reference to a valid source.</returns>
        Task<bool> Retrieve();

        /// <summary>
        /// Query whether the object exists at the target of the handle.
        /// If a retrieve was already attempted, this will return true or false depending on whether an object was found.
        /// Note: If the object has not been retrieved but the object has been set on the handle, this will still initiate
        /// a retrieve which if an object was found may result in a merge conflict (FUTURE).
        /// </summary>
        /// <seealso cref="TryGetObject"/>
        /// <returns>True if an object was found after a retrieval, false otherwise.</returns>
        Task<bool> Exists(bool forceCheck = false);
    }
}
