using LionFire.Events;
using System.ComponentModel;

namespace LionFire.Persistence
{
    public interface INotifyingReadHandle<T> 
        : IReadHandleBase<T>
        , INotifyingPersistence<IPersistenceSnapshot<T>>
        //, INotifyPersistence // Don't like
        //, INotifySenderChanged<IPersistenceSnapshot<T>>
        //, INotifySenderChanged<IPersistenceSnapshot<T>>
        //, INotifyChanged<T>
    {
    }

    public interface INotifyingReadHandleEx<T> 
        : INotifyingReadHandle<T>
        , INotifyPropertyChanged
    {
    }
    

    //public interface IEventedPersistenceReadHandle<out T> : IEventedReadHandle<T>, IPersisted { }
    //public interface ILazyRetrievableReadHandleCovariant<out T> : RH<T>, ILazilyGetsCovariant<T> { }
    //public interface ILazyRetrievableReadHandle<T> : RH<T>, ILazilyGets<T> { }




}
