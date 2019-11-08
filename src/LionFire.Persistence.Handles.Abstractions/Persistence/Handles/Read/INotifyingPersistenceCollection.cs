using LionFire.Collections;
using LionFire.Events;
using LionFire.Persistence.Handles;

namespace LionFire.Persistence
{
    public interface INotifyingPersistenceCollection<T> :
        IHandleBase,
        INotifyCollectionChanged<T>
    {
    }

    public interface INotifyingPersistenceCollectionEx<T> :
            INotifyingPersistence<T>,
            INotifyingPersistenceCollection<T>,
            INotifyCollectionChanged<T>
    {
    }

    //public interface IEventedPersistenceReadHandle<out T> : IEventedReadHandle<T>, IPersisted { }
    //public interface ILazyRetrievableReadHandleCovariant<out T> : RH<T>, ILazilyResolvesCovariant<T> { }
    //public interface ILazyRetrievableReadHandle<T> : RH<T>, ILazilyResolves<T> { }

}
