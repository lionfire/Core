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
    //    //public interface IPartialType<TValue> : IPartialType { }

    //    //public interface IItem { }
    //    //public interface IItem<TValue> : IItem { }


    public interface HC<out T, TListEntry> : IReadOnlyCollection<T>
        where TListEntry : ICollectionEntry
    {
        //        RH<INotifyingReadOnlyCollection<TValue>> Handle { get; }

        //        /// <summary>
        //        /// Direct data object
        //        /// </summary>
        //        INotifyingReadOnlyCollection<TListEntry> Entries { get; }

    }

    public interface HC<T> : RH<INotifyingReadOnlyCollection<H<T>>>
    {
        //        Task<int> Count();

        //        IPersistenceResult Add(string name, TValue obj);
        //        IPersistenceResult Add(H<TValue> handle);
        //        IPersistenceResult Add(TValue obj);

        //        IPersistenceResult Remove(string name);
        //        IPersistenceResult Remove(H<TValue> handle);
        //        IPersistenceResult Remove(TValue value);

        //        IEnumerable<CollectionOperation> UncommittedChanges { get; }

        //        IPersistenceResult Commit();
    }



    public interface RC<T, TListEntry> : IReadOnlyCollection<T>
        where TListEntry : ICollectionEntry
    {
        //        RH<INotifyingReadOnlyCollection<KeyValuePair<TListEntry,TValue>>> Handle { get; }

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
    /// A collection of objects found at a particular combining a for ReadOnly Collection in ObjectBus.  Inherits IReadOnlyCollection&lt;TValue&gt;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface RC<T> : RC<T, ICollectionEntry>
    {
    }
}
