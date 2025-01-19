using DynamicData;

namespace LionFire.Data.Collections;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// State:
///  - ObservableCache
///  
/// Implements:
///  - ReadCacheValue via ObservableCache
///  
/// Abstract:
///  - IObservable<IGetResult<>> IObservableGetOperations.GetOperations
/// </remarks>
public class PreresolvedAsyncObservableCollection<TKey, TValue>
    : DynamicDataCollection<TValue>
    , IAsyncReadOnlyKeyedCollection<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    #region Lifecycle

    public PreresolvedAsyncObservableCollection(IObservableCache<TValue, TKey> observableCache, AsyncObservableCollectionOptions? options = null)
    {
        ObservableCache = observableCache;
    }

    #endregion

    public bool IsReadOnly => true;

    #region State

    public IObservableCache<TValue, TKey> ObservableCache { get; private set; }

    #endregion

    #region Noop stubs

    #region State

    #region Methods

    public override void DiscardValue() => throw new NotSupportedException();

    #endregion

    #region Derived

    public override bool HasValue => true;

    public override IEnumerable<TValue>? ReadCacheValue => ObservableCache.Items;
    public override IEnumerable<TValue>? Value => this.ObservableCache.Items;

    #endregion

    #endregion

    #region Get

    public override ITask<IGetResult<IEnumerable<TValue>>> Get(CancellationToken cancellationToken = default)
        => Task.FromResult<IGetResult<IEnumerable<TValue>>>(GetResult<IEnumerable<TValue>>.NoopSuccess(this.ObservableCache.Items)).AsITask();

    public override void OnNext(IGetResult<IEnumerable<TValue>> result) { }

    #endregion

    #endregion

}
