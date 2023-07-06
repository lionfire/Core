using LionFire.Results;
using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Linq;

namespace LionFire.Data.Gets;

public interface ILazilyGets : IDefaultable
    , IDiscardableValue
    , IDiscardable
//, IHasPersistenceState
{

    // OLD
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
    

}


//public interface INotifyingLazilyResolves // Use Persistence instead?
//{
//    public event Action<ILazilyGets> Resolved;
//    public event Action<ILazilyGets> Discarded;
//}
