using LionFire.Referencing;
using LionFire.Resolvables;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persistence
{

    public interface RH : RH<object>
    {
    }

    public interface IReadHandleEvents<out T>
    {
        #region Events

        event Action<RH<T>, HandleEvents> HandleEvents;

        #endregion

        #region Events

        /// <summary>
        /// Raised if Object changes for any reason, such as: was retrieved from source, or was changed by user of handle.
        /// </summary>
        event Action<RH<T> /* handle */ , T /*oldValue*/ , T /*newValue*/> ObjectReferenceChanged;

        event Action<RH<T> /* handle */> ObjectChanged;

        #endregion
    }

    public interface IEventedReadHandle<out T> : RH<T>, IReadHandleEvents<T> { }
    public interface IPersistenceReadHandle<out T> : RH<T>, IHasPersistenceState { }
    public interface IEventedPersistenceReadHandle<out T> : IEventedReadHandle<T>, IHasPersistenceState { }

    public interface ILazyRetrievableReadHandle<out T> : RH<T>, ILazilyRetrievable { }

    public interface IReadHandleEx<out T> : RH<T>, IReadHandleEvents<T>, IHasPersistenceState, ILazilyRetrievable<T>, INotifyingWrapper<T> { }


    /// <summary>
    /// Interface for Read Handles.
    /// Features: 
    ///  - Resolves IReference to a value of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface RH<out T> : IReferencable, IRetrieves<T>
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
}
