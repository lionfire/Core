using System.Collections.Generic;
using System.Collections.Specialized;

namespace LionFire.Collections
{
    public interface INotifyCollectionChangedEventArgs<out T>
    {
        NotifyCollectionChangedAction Action { get; set; }
        IEnumerable<T> NewItems { get; }
        IEnumerable<T> OldItems { get; }

        NotifyCollectionChangedEventArgs ToNonGeneric();
    }
}