using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public interface IDetects
    {
        /// <summary>
        /// Query whether the object exists at the target of the handle.
        /// If a retrieve was already attempted, this will return true or false depending on whether an object was found.
        /// Note: If the object has not been retrieved but the object has been set on the handle, this will still initiate
        /// a retrieve which if an object was found may result in a merge conflict (FUTURE).
        /// </summary>
        /// <seealso cref="TryGetObject"/>
        /// <returns>True if an object was found after a retrieval, false otherwise.</returns>
        Task<bool> Exists(bool forceCheck = false); // REVIEW - pass to ICanResolveAsync, like with ResolveAsync?
    }

    public static class IDetectsFallback
    {

        //public static async Task<bool> Exists<T>(this IRetrieves<T> retrieves, bool forceCheck = false)
        //{
        //    throw new NotImplementedException("This needs more thought and logic");
        //    //if(retrieves is IDetects detects) { return await detects.Exists(forceCheck).ConfigureAwait(false);  }
        //    //if(retrieves is IHasPersistenceState hps)
        //    //{
        //    //    if (hps.NotFound()) return false;
        //    //    if (hps.State.HasFlag(PersistenceResultFlags.Found)) return true;
        //    //}
        //}
    }
}
