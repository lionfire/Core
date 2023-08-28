using DynamicData;
using LionFire.Data.Async;
using LionFire.Data.Async.Gets;
using MorseCode.ITask;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Subjects;
namespace LionFire.Data.Collections;


// TODO: Rename AsyncDynamicDataCollection to AsyncDynamicDataCollectionRxO, and have Lightweight version of AsyncDynamicDataCollection? It would possibly be without
//  - ReactiveObject base
//  - Value property

public abstract partial class AsyncLazyDynamicDataCollection<TValue> : AsyncDynamicDataCollection<TValue>
{
    public override void DiscardValue() => ReadCacheValue = Enumerable.Empty<TValue>();

    public override IEnumerable<TValue>? ReadCacheValue { get; protected set; }

    public override async ITask<IGetResult<IEnumerable<TValue>>> Get(CancellationToken cancellationToken = default)
    {
        // TODO: THREADSAFETY, see code from non-collection 

        var t = await GetImpl(cancellationToken).ConfigureAwait(false);

        if (t.IsSuccess())
        {
            ReadCacheValue = t.Value;
        }
        else
        {
            // TODO: if Discard cache on Fail, discard it here
        }
        return t;
    }

    public abstract ITask<IGetResult<IEnumerable<TValue>>> GetImpl(CancellationToken cancellationToken = default);
}

/// <summary>
/// Description: local cache of a collection that is read (and maybe written) across an async boundary (such as network or disk)
///   
/// This is built for DynamicData's 2 collection types:
///  - for DynamicData's ObservableCache<TValue, TKey>:
///    - AsyncReadOnlyDictionaryCache<TKey, TValue>
///      - AsyncDictionaryCache<TKey, TValue>
///  - for DynamicData's ObservableList<TValue>:
///    - AsyncReadOnlyListCache<TValue>
///      - AsyncListCache<TValue>
/// 
/// </summary>
/// <typeparam name="TValue"></typeparam>
public abstract partial class AsyncDynamicDataCollection<TValue>
    : ReactiveObject
    , IEnumerableGetter<TValue>
    , IObservableGetOperations<IEnumerable<TValue>>
// Derived classes may implement read interfaces:
//  - INotifiesChildChanged
//  - INotifiesChildDeeplyChanged

// Derived classes may implement write interfaces:
//  - IAsyncCreates<TItem>
//  - System.IAsyncObserver<ChangeSet<TItem>> // For subscribing to changes
{

    #region IAsyncReadOnlyCollectionCache<TItem>

    #region IObservableGetOperations

    //[Obsolete("TODO: Move to VM class")]
    //public bool IsResolving => !getOperations.Value.AsTask().IsCompleted;

    public IObservable<ITask<IGetResult<IEnumerable<TValue>>>> GetOperations => getOperations;
    protected BehaviorSubject<ITask<IGetResult<IEnumerable<TValue>>>> getOperations = new(Task.FromResult<IGetResult<IEnumerable<TValue>>>(NoopNotFoundGetResult<IEnumerable<TValue>>.Instance).AsITask());

    #region IResolves

    /// <summary>
    /// The DynamicData collection should be patched or replaced with the contents of the IEnumerable.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract ITask<IGetResult<IEnumerable<TValue>>> Get(CancellationToken cancellationToken = default);

    #endregion

    #endregion

    #region ILazilyGets<IEnumerable<TItem>>

    #region State

    public abstract IEnumerable<TValue>? Value { get; }

    #endregion

    public virtual bool HasValue => ReadCacheValue != null;

    #region IReadWrapper<T>

    public abstract IEnumerable<TValue>? ReadCacheValue { get; protected set; }
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

    private GetterOptions DefaultOptions => GetterOptions<IEnumerable<TValue>>.Default;

    #endregion

    #region Methods

    public async ITask<IGetResult<IEnumerable<TValue>>> GetIfNeeded()
    {
        // TODO ENH - Same read Semaphore as AsyncGet<TValue>
        if (HasValue) { return new NoopGetResult<IEnumerable<TValue>>(HasValue, ReadCacheValue); }
        var result = await Get().ConfigureAwait(false);
        return new GetResult<IEnumerable<TValue>>(result.Value, result.HasValue);
    }

    /// <summary>
    /// Get HasValue and ReadCacheValue (or Value) combined in one IGetResult
    /// </summary>
    /// <returns></returns>
    public virtual IGetResult<IEnumerable<TValue>> QueryValue() => new NoopGetResult<IEnumerable<TValue>>(HasValue, ReadCacheValue);

    #endregion

    #region Discard

    public abstract void DiscardValue(); // => Value = null;
    public virtual void Discard() => DiscardValue();

    #endregion

    #endregion

    // REVIEW - should this be brought back?  Count and GetEnumerator()?
    //#region IReadOnlyCollection<TItem>

    //public virtual int Count => (ReadCacheValue ?? Enumerable.Empty<TValue>()).Count();

    //#region IEnumerable<TItem>

    //[Blocking(Alternative = nameof(GetIfNeeded))]
    //public virtual IEnumerator<TValue> GetEnumerator() => (ReadCacheValue ?? Enumerable.Empty<TValue>()).GetEnumerator();
    //IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    //#endregion

    //#endregion

    #endregion

    //public abstract DynamicData.IObservableList<TItem> List { get; }
}
