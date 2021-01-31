using LionFire.Collections;
using System.Collections.Generic;

namespace LionFire.Persistence
{
    // TODO: Deprecate IReadOnlyCollection in favor of IReadOnlyList
    public interface HC<T, TListEntry> : HC<T>, IReadOnlyCollection<T>, IAsyncEnumerable<T>
         where TListEntry : ICollectionEntry
    {
        //        RH<INotifyingReadOnlyCollection<T>> Handle { get; }

        //        /// <summary>
        //        /// Direct data object
        //        /// </summary>
        //        INotifyingReadOnlyCollection<TListEntry> Entries { get; }
    }
    
    public interface HC<T> : IReadHandleBase<INotifyingReadOnlyCollection<IReadWriteHandle<T>>>
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

        IReadWriteHandle<T> this[string subpath] { get; }
    }
}
