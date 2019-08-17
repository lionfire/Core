using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public interface ILazyRetrievable
    {
        /// <summary>
        /// Invokes get_Object, forcing a lazy retrieve if it was null and State does not have the NotFound flag (TOTEST).
        /// If the user has set the object, then this will return true even if the object is not committed back to the source yet.
        /// </summary>
        /// <seealso cref="Exists"/>
        /// <returns>True if an object was found after a retrieval or was manually set on the handle, false otherwise (in which case State is set to |= PersistenceState.NotFound, if it doesn't already have that flag (TOTEST)).</returns>
        Task<(bool HasObject, T Object)> Get<T>();

    }
}
