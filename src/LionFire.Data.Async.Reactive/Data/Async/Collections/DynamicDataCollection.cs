using LionFire.ExtensionMethods;
using System.Collections;
using System.Reactive.Subjects;
namespace LionFire.Data.Collections;

/// <summary>
/// Description: local cache of a collection that is read (and maybe written) across an async boundary (such as network or disk)
///   
/// This is built for DynamicData's 2 collection types:
///  - for DynamicData's ObservableCache<TValue, TKey>:
///    - AsyncReadOnlyDictionary<TKey, TValue>
///      - AsyncDictionary<TKey, TValue>
///  - for DynamicData's ObservableList<TValue>:
///    - AsyncReadOnlyList<TValue>
///      - AsyncList<TValue>
/// 
/// </summary>
/// <typeparam name="TValue"></typeparam>
public abstract partial class DynamicDataCollection<TValue>
    : ReactiveObject
    , IEnumerableGetter<TValue>
    , IObservableGetOperations<IEnumerable<TValue>>
    , IGetterRxO<IEnumerable<TValue>>
    , IObserver<IGetResult<IEnumerable<TValue>>>
    
// Derived classes may implement read interfaces:
//  - INotifiesChildChanged
//  - INotifiesChildDeeplyChanged

// Derived classes may implement write interfaces:
//  - IAsyncCreates<TItem>
//  - System.IAsyncObserver<ChangeSet<TItem>> // For subscribing to changes
{

    #region Lifecycle

    public DynamicDataCollection() {

        GetOperations.Subscribe((Action<ITask<IGetResult<IEnumerable<TValue>>>>)(async t =>
        {
            var o = (IObserver<IGetResult<IEnumerable<TValue>>>)this;
            try
            {
                var result = await t.ConfigureAwait(false);
                Debug.WriteLine($"DynamicDataCollection.GetOperations.OnNext: {result.ToDebugString()}");
                o.OnNext(result);
            }
            catch(Exception ex)
            {
                o.OnError(ex);
            }

            //if (result.IsSuccess())
            //{
            //    ReadCacheValue = result.Value;
            //    ((IReactiveObject)this).RaisePropertyChanged(nameof(ReadCacheValue));
            //    if (ValueSourceIsReadCacheValue)
            //    {
            //        ((IReactiveObject)this).RaisePropertyChanged(nameof(Value)); // TODO: ValueSourceIsReadCacheValue
            //    }
            //}
            //else
            //{
            //}
        }));
    }
    
    #endregion

    #region IAsyncReadOnlyCollectionCache<TItem>

    #region IObservableGetOperations

    //[Obsolete("TODO: Move to VM class")]
    //public bool IsResolving => !getOperations.Value.AsTask().IsCompleted;

    public IObservable<ITask<IGetResult<IEnumerable<TValue>>>> GetOperations => getOperations;
    protected BehaviorSubject<ITask<IGetResult<IEnumerable<TValue>>>> getOperations = new(Task.FromResult<IGetResult<IEnumerable<TValue>>>(InitializedGetResult<IEnumerable<TValue>>.Instance).AsITask());

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
    public virtual bool ValueSourceIsReadCacheValue => true;

    #endregion

    public virtual bool HasValue => ReadCacheValue != null;

    public virtual IEnumerator<TValue> GetEnumerator() => (ReadCacheValue ?? Enumerable.Empty<TValue>()).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    #region IReadWrapper<T>

    public abstract IEnumerable<TValue>? ReadCacheValue { get; }
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
        if (HasValue) { return GetResult<IEnumerable<TValue>>.NoopSyncSuccess(ReadCacheValue); }
        var result = await Get().ConfigureAwait(false);
        return result;
        //return new GetResult<IEnumerable<TValue>>(result.Value, result.HasValue) { Flags = result.Flags };
    }

    /// <summary>
    /// Get HasValue and ReadCacheValue (or Value) combined in one IGetResult
    /// </summary>
    /// <returns></returns>
    public virtual IGetResult<IEnumerable<TValue>> QueryGetResult() => new NoopGetResult<IEnumerable<TValue>>(HasValue, ReadCacheValue);

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

    #region IObservable GetResults

    public virtual void OnCompleted() { }

    public virtual void OnError(Exception error)
    {
        Debug.WriteLine($"{this.GetType().ToHumanReadableName()} get exception: {error}");
    }

    public abstract void OnNext(IGetResult<IEnumerable<TValue>> result);

    #endregion

}
