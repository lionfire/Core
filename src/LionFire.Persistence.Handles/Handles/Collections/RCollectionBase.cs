using System;
using System.Collections;
using System.Collections.Generic;
using LionFire.Collections;
using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{

    public abstract class RCollectionBase<TReference, TCollection, TItem> : ReadHandle<TReference, TCollection>, IReadOnlyCollection<TItem>
        where TReference : IReference
        where TCollection : class, IEnumerable<TItem>
    {
        public abstract int Count { get; }

        public abstract IEnumerator<TItem> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

        public abstract void OnCollectionChangedEvent(INotifyCollectionChangedEventArgs<TItem> a);
    }

#if Commented
    public abstract class HCollectionBase<ObjectType, T> : WBase<ObjectType>, ICollection<T>
        where ObjectType : IEnumerable<T>
    {
        public abstract int Count { get; }
        public abstract bool IsReadOnly { get; }

        public abstract void Add(T item);
        public abstract bool Remove(T item);
        public abstract void Clear();

        public abstract bool Contains(T item);

        public abstract void CopyTo(T[] array, int arrayIndex);

        public abstract IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }

    public class WritableCollectionExperiment<T> : HCollectionBase<List<T>, T>
    {
        MultiBindableCollection<T> collection = new MultiBindableCollection<T>();

        public override int Count => collection.Count;

        public override bool IsReadOnly => collection.IsReadOnly;

        public override void Add(T item) => collection.Add(item);
        public override void Clear() => collection.Clear();
        public override bool Contains(T item) => collection.Contains(item);
        public override void CopyTo(T[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);
        public override Task DeleteObject(object persistenceContext = null) => collection.DeleteObject(persistenceContext);
        public override IEnumerator<T> GetEnumerator() => collection.GetEnumerator();
        public override bool Remove(T item) => collection.Remove();
        public override Task<bool> TryRetrieveObject() => collection.TryRetrieveObject();
        public override Task WriteObject(object persistenceContext = null) => collection.WriteObject(persistenceContext);
    }
#endif

}
