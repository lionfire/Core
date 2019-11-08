using LionFire.Structures;

namespace LionFire.Resolves
{

    public interface ILazilyResolves : IWrapper, IDiscardableValue
    //: IHasPersistenceState
    {
        /// <summary>
        /// Lazily gets the Object, meaning if the Object is not known yet and a retrieve has not been performed, it will be retrieved via the IRetrieves interface.  If the Object is already known, it avoids a Retrieve.  (For writable objects, the Object might be known because the user set the Object.)
        /// 
        /// To force a Retrieve, either invoke IRetrieves.Retrieve if supported by this object, or else invoke DiscardValue followed by Get.
        /// </summary>
        /// <seealso cref="Exists"/>
        /// <returns>True if an object was found after a retrieval or was manually set on the handle, false otherwise (in which case State is set to |= PersistenceState.NotFound, if it doesn't already have that flag (TOTEST)).</returns>
        /// <remarks>
        /// If the user has set the object, then this will return HasObject: true even if the object is not committed back to the source yet.
        /// </remarks>
        //Task<(bool success, IPersistableSnapshot<T> result)> Get<T>();


        ///// <summary>
        ///// Returns true if retrieval was attempted via get_Object or TryResolveObject, and a non-null object was retrieved. 
        ///// (TODO: treat null as a valid object that can be retrieved, for nullable types.)
        ///// Use this to test if the object has been retrieved without attempting a retrieve.
        ///// </summary>
        //bool HasObject { get; }

        //bool RetrievedNull { get; }

        /// <summary>
        /// Determine whether this object has a Value without triggering any lazy resolving of the value.
        /// </summary>
        new bool HasValue { get; }

    }

    //public interface INotifyingLazilyResolves // Use Persistence instead?
    //{
    //    public event Action<ILazilyResolves> Resolved;
    //    public event Action<ILazilyResolves> Discarded;
    //}
}
