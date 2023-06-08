using DynamicData;
using LionFire.Data.Async.Gets;
using MorseCode.ITask;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections;
using System.Reactive.Subjects;

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
public abstract partial class AsyncDynamicDataCollectionCache<TValue>
    : ReactiveObject
    , IAsyncReadOnlyCollectionCacheBase<TValue>
    , IObservableResolves<IEnumerable<TValue>>
// Derived classes may implement read interfaces:
//  - INotifiesChildChanged
//  - INotifiesChildDeeplyChanged

// Derived classes may implement write interfaces:
//  - IAsyncCreates<TItem>
//  - System.IAsyncObserver<ChangeSet<TItem>> // For subscribing to changes
{

    #region IAsyncReadOnlyCollectionCache<TItem>

    #region IObservableResolves

    public bool IsResolving => !resolves.Value.AsTask().IsCompleted;
    public IObservable<ITask<IResolveResult<IEnumerable<TValue>>>> Resolves => resolves;
    protected BehaviorSubject<ITask<IResolveResult<IEnumerable<TValue>>>> resolves = new(Task.FromResult<IResolveResult<IEnumerable<TValue>>>(ResolveResultNotResolvedNoop<IEnumerable<TValue>>.Instance).AsITask());

    #region IResolves

    public abstract ITask<IResolveResult<IEnumerable<TValue>>> Resolve();

    #endregion

    #endregion

    #region ILazilyResolves<IEnumerable<TItem>>

    public async ITask<ILazyResolveResult<IEnumerable<TValue>>> TryGetValue()
    {
        if (HasValue) { return new LazyResolveNoopResult<IEnumerable<TValue>>(HasValue, Value); }
        var result = await Resolve().ConfigureAwait(false);
        return new LazyResolveResult<IEnumerable<TValue>>(result.HasValue, result.Value);
    }

    public virtual ILazyResolveResult<IEnumerable<TValue>> QueryValue() => new LazyResolveNoopResult<IEnumerable<TValue>>(HasValue, Value);

    public virtual bool HasValue => Value != null;

    public abstract void DiscardValue(); // => Value = null;

    #region IReadWrapper<T>

    public abstract IEnumerable<TValue>? Value { get; }

    #endregion

    #endregion

    #region IReadOnlyCollection<TItem>

    public virtual int Count => (Value ?? Enumerable.Empty<TValue>()).Count();

    #region IEnumerable<TItem>

    public virtual IEnumerator<TValue> GetEnumerator() => (Value ?? Enumerable.Empty<TValue>()).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #endregion

    #endregion

    //public abstract DynamicData.IObservableList<TItem> List { get; }

}
