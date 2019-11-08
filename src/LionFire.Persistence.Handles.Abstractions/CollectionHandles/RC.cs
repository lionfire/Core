using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Collections;
using LionFire.Persistence.Collections;
using LionFire.Referencing;

namespace LionFire.Persistence
{
    //    //public interface IPartialType { }
    //    //public interface IPartialType<T> : IPartialType { }

    //    //public interface IItem { }
    //    //public interface IItem<T> : IItem { }


    public interface HC<out T, TListEntry> : IReadOnlyCollection<T>
        where TListEntry : ICollectionEntry
    {
        //        RH<INotifyingReadOnlyCollection<T>> Handle { get; }

        //        /// <summary>
        //        /// Direct data object
        //        /// </summary>
        //        INotifyingReadOnlyCollection<TListEntry> Entries { get; }

    }

    public interface HC<T> : IReadHandleBase<INotifyingReadOnlyCollection<IReadWriteHandleBase<T>>>
    {
        //        Task<int> Count();

        //        IPersistenceResult Add(string name, T obj);
        //        IPersistenceResult Add(H<T> handle);
        //        IPersistenceResult Add(T obj);

        //        IPersistenceResult Remove(string name);
        //        IPersistenceResult Remove(H<T> handle);
        //        IPersistenceResult Remove(T value);

        //        IEnumerable<CollectionOperation> UncommittedChanges { get; }

        //        IPersistenceResult Put();
    }



    public interface RC<T, TListEntry> : IReadOnlyCollection<T>
        where TListEntry : ICollectionEntry
    {
        //        RH<INotifyingReadOnlyCollection<KeyValuePair<TListEntry,T>>> Handle { get; }

        //        ///// <summary>
        //        ///// Direct data object
        //        ///// </summary>
        //        //INotifyingReadOnlyCollection<TListEntry> Entries { get; }

    }

    ///// <summary>
    ///// This non-generic is just an alternative to IRC&lt;object&gt;  (Not recommended in most APIs)
    ///// </summary>
    //public interface RC : RC<object> { }


    /// <summary>
    /// A collection of objects found at a particular combining a for ReadOnly Collection in ObjectBus.  Inherits IReadOnlyCollection&lt;T&gt;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface RC<T> : RC<T, ICollectionEntry>
    {
    }
}
