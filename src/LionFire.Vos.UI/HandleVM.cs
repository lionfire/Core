using LionFire.Persistence;

namespace LionFire.Vos.UI
{
    public class HandleVM<T>
    {
        protected object Handle { get; }
        protected virtual void OnHandleChanged(object value)
        {
            if(value is INotifyPersists<T> np)
            {
                np.PersistenceStateChanged += OnPersistenceStateChanged;
            }
        }

        protected virtual void OnPersistenceStateChanged(PersistenceEvent<T> obj) { }
    }
}
