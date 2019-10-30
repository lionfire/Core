using LionFire.Events;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolvables;
using LionFire.Resolves;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
#if false // Not needed with ITask?
    public interface IReadHandleInvariant<T> : RH<T> { }
    public interface IReadHandleInvariantEx<T> : IReadHandleInvariant<T>, INotifyingPersisted<T> { }
#endif

    /// <summary>
    /// IReadHandle - Minimal interface for Read Handles.  (See also: IReadHandleEx)
    /// 
    /// Features: 
    ///  - Resolves IReference to a value of type T
    ///  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface RH<out T> : IHandleBase, IRetrieves<T>
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

    // INotifyPersistence - older
#error NEXT: Flesh out these interfaces and which primary notification interface I want, and which ones I want as Ex.
    public interface INotifyingPersistence<out T> : 
        IHandleBase, 
        INotifyChanged<IPersistenceSnapshot<T>>,
        INotifySenderChanged<IPersistenceSnapshot<T>>
    {

    }
    public interface INotifyingPersistenceEx<out T> :
        INotifyingPersistence<T>,
        INotifyPropertyChanged
    {

    }

    public interface INotifyingPersistenceCollection<out T> :
        IHandleBase,
        INotifyChanged<IPersistenceSnapshot<T>>,
        INotifySenderChanged<IPersistenceSnapshot<T>>
    {

    }
    public interface INotifyingPersistenceCollectionEx<out T> :
            INotifyingPersistenceCollection<T>,
            INotifyCollectionChanged
    {

    }


    public interface INotifyingReadHandleEx<out T> : INotifyingReadHandle<T>, INotifyPropertyChanged
    {
    }

    public interface INotifyingReadHandle<out T> : IReadHandleEx<T>
        //, INotifyPersistence // Don't like
        , INotifyChanged<IPersistenceSnapshot<T>>
        , INotifySenderChanged<IPersistenceSnapshot<T>>
        , INotifyChanged<T>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadHandleEx<out T> : RH<T>, IHandleEx, IPersisted, ILazilyResolves<T>, INotifyChanged<T>, INotifyChanged<T>
    //, INotifyingWrapper<T>
    //IReadHandleEvents<T>,
    {
    }
    
#region OLD 

#region RH

    // UNUSED - may be needed for AOT and/or Unity if there is still a bug with covariant generics, but I think that has probably been fixed by now
    ///// <summary>
    ///// Eliminate this?
    ///// </summary>
    //[Obsolete("Use RH<object> instead")]
    //public interface RH : RH<object>
    //{
    //}

#endregion



#region IReadHandleEvents

    //public interface IReadHandleEvents<out T>
    //{
    //    #region Events

    //    event Action<RH<T>, HandleEvents> HandleEvents;

    //    #endregion

    //    #region Events

    //    /// <summary>
    //    /// Raised if Object changes for any reason, such as: was retrieved from source, or was changed by user of handle.
    //    /// </summary>
    //    event Action<RH<T> /* handle */ , T /*oldValue*/ , T /*newValue*/> ObjectReferenceChanged;

    //    event Action<RH<T> /* handle */> ObjectChanged;

    //    #endregion
    //}
#endregion

    //public interface IEventedReadHandle<out T> : RH<T>, IReadHandleEvents<T> { }
    public interface IPersistenceReadHandle<out T> : RH<T>, IPersisted { }
    //public interface IEventedPersistenceReadHandle<out T> : IEventedReadHandle<T>, IPersisted { }

    //public interface ILazyRetrievableReadHandleCovariant<out T> : RH<T>, ILazilyResolvesCovariant<T> { }
    //public interface ILazyRetrievableReadHandle<T> : RH<T>, ILazilyResolves<T> { }

#endregion



}
