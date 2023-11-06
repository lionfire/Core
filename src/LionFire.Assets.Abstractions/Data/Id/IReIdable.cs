#if UNUSED
using System;
using LionFire.Structures;

namespace LionFire.Data.Id
{
    /// <summary>
    /// Potentially useful when you want to be able to rename your objects and they are identified by their name.
    /// </summary>
    /// <remarks>
    /// Use caution when designing for mutable primary keys.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    [Obsolete("Use IIdentifiable in combination with a new INotifyingIdentifiable for event handling")]
    public interface IReIdable<T, TKey> 

    {
        void ReId(TKey newId);
        event Action<IIdentified<TKey>, TKey, TKey> IdChangedFromTo;
    }
}
#endif