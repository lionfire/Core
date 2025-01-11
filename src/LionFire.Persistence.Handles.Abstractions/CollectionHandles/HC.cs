using LionFire.Collections;
using System.Collections.Generic;

namespace LionFire.Persistence;

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

    //        ITransferResult Add(string name, T obj);
    //        ITransferResult Add(H<T> handle);
    //        ITransferResult Add(T obj);

    //        ITransferResult Remove(string name);
    //        ITransferResult Remove(H<T> handle);
    //        ITransferResult Remove(T value);

    //        IEnumerable<CollectionOperation> UncommittedChanges { get; }

    //        ITransferResult Put();

    IReadWriteHandle<T> this[string subpath] { get; }
}
