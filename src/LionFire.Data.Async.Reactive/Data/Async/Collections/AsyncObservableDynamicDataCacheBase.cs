using DynamicData;
using LionFire.Data.Gets;
using MorseCode.ITask;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Subjects;
namespace LionFire.Data.Collections;


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

    public bool IsResolving => !gets.Value.AsTask().IsCompleted;
    public IObservable<ITask<IGetResult<IEnumerable<TValue>>>> Gets => gets;
    protected BehaviorSubject<ITask<IGetResult<IEnumerable<TValue>>>> gets = new(Task.FromResult<IGetResult<IEnumerable<TValue>>>(ResolveResultNotResolvedNoop<IEnumerable<TValue>>.Instance).AsITask());

    #region IResolves

    public abstract ITask<IGetResult<IEnumerable<TValue>>> Get(CancellationToken cancellationToken = default);

    #endregion

    #endregion

    #region ILazilyGets<IEnumerable<TItem>>

    #region State

    public abstract IEnumerable<TValue>? ReadCacheValue { get;  }

    #endregion

    public virtual bool HasValue => ReadCacheValue != null;

    #region IReadWrapper<T>

    public abstract IEnumerable<TValue>? Value { get; }
    //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    //public IEnumerable<TValue> Value
    //{
    //    [Blocking(Alternative = nameof(GetIfNeeded))]
    //    get
    //    {            
    //        Debugger.NotifyOfCrossThreadDependency();
    //        return ReadCacheValue ?? (DefaultOptions.BlockToGet ? GetIfNeeded().Result.Value ?? Enumerable.Empty<TValue>() : Enumerable.Empty<TValue>());
    //    }
    //}

    private AsyncGetOptions DefaultOptions => AsyncGetOptions<IEnumerable<TValue>>.Default;

    #endregion

    #region Methods

    public async ITask<ILazyGetResult<IEnumerable<TValue>>> GetIfNeeded()
    {
        // TODO ENH - Same read Semaphore as AsyncGet<TValue>
        if (HasValue) { return new LazyResolveNoopResult<IEnumerable<TValue>>(HasValue, ReadCacheValue); }
        var result = await Get().ConfigureAwait(false);
        return new LazyResolveResult<IEnumerable<TValue>>(result.HasValue, result.Value);
    }

    public virtual ILazyGetResult<IEnumerable<TValue>> QueryValue() => new LazyResolveNoopResult<IEnumerable<TValue>>(HasValue, ReadCacheValue);

    #endregion

    #region Discard

    public abstract void DiscardValue(); // => Value = null;
    public virtual void Discard() => DiscardValue();

    #endregion

    #endregion

    #region IReadOnlyCollection<TItem>

    public virtual int Count => (Value ?? Enumerable.Empty<TValue>()).Count();

    #region IEnumerable<TItem>

    [Blocking(Alternative = nameof(GetIfNeeded))]
    public virtual IEnumerator<TValue> GetEnumerator() => (Value ?? Enumerable.Empty<TValue>()).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #endregion

    #endregion

    //public abstract DynamicData.IObservableList<TItem> List { get; }

}
