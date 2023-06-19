using DynamicData;
using LionFire.Data.Async.Gets;
using MorseCode.ITask;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections;
using System.Reactive.Subjects;
namespace LionFire.Data.Async.Collections;


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
    , IObservableGets<IEnumerable<TValue>>
// Derived classes may implement read interfaces:
//  - INotifiesChildChanged
//  - INotifiesChildDeeplyChanged

// Derived classes may implement write interfaces:
//  - IAsyncCreates<TItem>
//  - System.IAsyncObserver<ChangeSet<TItem>> // For subscribing to changes
{

    #region IAsyncReadOnlyCollectionCache<TItem>

    #region IObservableGets

    public bool IsResolving => !resolves.Value.AsTask().IsCompleted;
    public IObservable<ITask<IGetResult<IEnumerable<TValue>>>> Gets => resolves;
    protected BehaviorSubject<ITask<IGetResult<IEnumerable<TValue>>>> resolves = new(Task.FromResult<IGetResult<IEnumerable<TValue>>>(ResolveResultNotResolvedNoop<IEnumerable<TValue>>.Instance).AsITask());

    #region IResolves

    public abstract ITask<IGetResult<IEnumerable<TValue>>> Get();

    #endregion

    #endregion

    #region ILazilyGets<IEnumerable<TItem>>

    public async ITask<ILazyGetResult<IEnumerable<TValue>>> TryGetValue()
    {
        if (HasValue) { return new LazyResolveNoopResult<IEnumerable<TValue>>(HasValue, Value); }
        var result = await Get().ConfigureAwait(false);
        return new LazyResolveResult<IEnumerable<TValue>>(result.HasValue, result.Value);
    }

    public virtual ILazyGetResult<IEnumerable<TValue>> QueryValue() => new LazyResolveNoopResult<IEnumerable<TValue>>(HasValue, Value);

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
