using LionFire.Data.Async.Gets;
using LionFire.Persistence.Handles;

namespace LionFire.Persistence;

/// <summary>
/// IReadHandle - Minimal interface for Read Handles.  (See also: IReadHandleEx)
/// 
/// Features: 
///  - Resolves IReference to a value of type T
///  
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IReadHandleBase<out T> : IHandleBase, ILazilyGets<T>
{
    ///// <summary>
    ///// Returns true if retrieval was attempted via get_Object or TryResolveObject, and a non-null object was retrieved.
    ///// </summary>
    //new bool HasObject { get; }

    //public static bool ForgetObjectOnRetrieveFail = false; // FUTURE?

    #region Retrieve

    /// <summary>
    /// REVIEW 
    /// Invokes get_Object, forcing a lazy retrieve if it was null and State does not have the NotFound flag (TOTEST).
    /// If the user has set the object, then this will return true even if the object is not committed back to the source yet.
    /// </summary>
    /// <seealso cref="Exists"/>
    /// <returns>True if an object was found after a retrieval or was manually set on the handle, false otherwise (in which case State is set to |= PersistenceState.NotFound, if it doesn't already have that flag (TOTEST)).</returns>
    //Task<(bool success, T obj)> GetObject();        

    #endregion
}
