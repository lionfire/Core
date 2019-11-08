using LionFire.Events;
using LionFire.Persistence.Handles;
using System.ComponentModel;

namespace LionFire.Persistence
{
    public interface INotifyingPersistence<T> : 
        IHandleBase, 
        INotifyChanged<IPersistenceSnapshot<T>>,
        INotifySenderChanged<IPersistenceSnapshot<T>>
    {
    }

    public interface INotifyingPersistenceEx<T> :
            INotifyingPersistence<T>,
            INotifyPropertyChanged
    {
    }


}
