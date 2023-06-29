using LionFire.Collections;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolvables;
using LionFire.Data.Gets;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    /// <summary>
    /// Limited interface for when generic interface type is not available
    /// </summary>
    public interface IReadHandle : IHandleEx, IPersists, ILazilyGets, IDefaultable, IGets
    {
    }

    /// <summary>
    /// Lazy Read Persistence Handle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadHandle<out T> : IReadHandleBase<T>, IReadHandle, ILazilyGets<T>
        , IReferencableAsValueType<T>
    // , INotifyChanged<T>
    //, INotifyingWrapper<T>
    //IReadHandleEvents<T>,
    {
    }

    #region OLD 

#if false // Not needed with ITask?
    public interface IReadHandleInvariant<T> : RH<T> { }
    public interface IReadHandleInvariantEx<T> : IReadHandleInvariant<T>, INotifyingPersisted<T> { }
#endif

    //public interface IEventedPersistenceReadHandle<out T> : IEventedReadHandle<T>, IPersisted { }

    //public interface ILazyRetrievableReadHandleCovariant<out T> : RH<T>, ILazilyResolvesCovariant<T> { }
    //public interface ILazyRetrievableReadHandle<T> : RH<T>, ILazilyGets<T> { }


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
    public interface IPersistenceReadHandle<out T> : IReadHandleBase<T>, IPersists { }
    //public interface IEventedPersistenceReadHandle<out T> : IEventedReadHandle<T>, IPersisted { }

    //public interface ILazyRetrievableReadHandleCovariant<out T> : RH<T>, ILazilyResolvesCovariant<T> { }
    //public interface ILazyRetrievableReadHandle<T> : RH<T>, ILazilyGets<T> { }

#endregion

}
