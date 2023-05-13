using DynamicData;
using LionFire.Resolves;
using MorseCode.ITask;
using ReactiveUI;
using System.Collections;

namespace LionFire.Collections.Async;


// Description: local cache of a collection that is read (and maybe written) across an async boundary (such as network or disk)
//   
// This is built for DynamicData's 2 collection types:
//  - for DynamicData's ObservableCache<TValue, TKey>:
//    - AsyncReadOnlyDictionaryCache<TKey, TValue>
//      - AsyncDictionaryCache<TKey, TValue>
//  - for DynamicData's ObservableList<TValue>:
//    - AsyncReadOnlyListCache<TValue>
//      - AsyncListCache<TValue>
public abstract partial class AsyncDynamicDataCollectionCache<TItem>
    : ReactiveObject
    , IAsyncReadOnlyCollectionCache<TItem>
// Derived classes may implement read interfaces:
//  - INotifiesChildChanged
//  - INotifiesChildDeeplyChanged

// Derived classes may implement write interfaces:
//  - IAsyncCreates<TItem>
//  - System.IAsyncObserver<ChangeSet<TItem>> // For subscribing to changes
{

    #region IAsyncReadOnlyCollectionCache<TItem>

    #region IObservableResolves

    public abstract IObservable<ITask<IResolveResult<IEnumerable<TItem>>>> Resolves { get; }

    #region IResolves

    public abstract ITask<IResolveResult<IEnumerable<TItem>>> Resolve();

    #endregion

    #endregion

    #region ILazilyResolves<IEnumerable<TItem>>

    public abstract ITask<ILazyResolveResult<IEnumerable<TItem>>> TryGetValue();
    public abstract ILazyResolveResult<IEnumerable<TItem>> QueryValue();

    public abstract bool HasValue { get; }

    public abstract void DiscardValue();

    #region IReadWrapper<T>

    public abstract IEnumerable<TItem>? Value { get; }

    #endregion

    #endregion

    #region IReadOnlyCollection<TItem>

    public abstract int Count { get; }

    #region IEnumerable<TItem>

    public abstract IEnumerator<TItem> GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #endregion

    #endregion

    public abstract DynamicData.IObservableList<TItem> List { get; }

}
