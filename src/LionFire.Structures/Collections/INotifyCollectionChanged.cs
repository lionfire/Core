using System.Collections.Specialized;

namespace LionFire.Collections
{
    //public delegate void NotifyCollectionChangedHandler<T> (NotifyCollectionChangedEventArgs<T> collection, NotifyCollectionChangedEventArgs<T> args);
    //#if AOT
    //	public delegate void NotifyCollectionChangedHandler2<TValue>(NotifyCollectionChangedEventArgs<TValue> e);
    //#endif

    public delegate void NotifyCollectionChangedHandler<T>(NotifyCollectionChangedEventArgs<T> e);

    public interface INotifyCollectionChanged<ChildType>
#if AOT
 : INotifyCollectionChanged
#endif
    {
#if AOT
#else
        event NotifyCollectionChangedHandler<ChildType> CollectionChanged;
#endif
    }

}
