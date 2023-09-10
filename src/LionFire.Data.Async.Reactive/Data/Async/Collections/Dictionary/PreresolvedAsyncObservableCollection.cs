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
    : AsyncDynamicDataCollection<KeyValuePair<TKey, TValue>>
    , IAsyncReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    #region Lifecycle

    public PreresolvedAsyncObservableCollection(IObservableCache<TValue, TKey> observableCache, AsyncObservableCollectionOptions? options = null)
    {
        ObservableCache = observableCache;
    }

    #endregion

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

    public override IEnumerable<KeyValuePair<TKey, TValue>>? ReadCacheValue
    {
        get => ObservableCache.KeyValues;
        protected set => throw new NotSupportedException();
    }
    public override IEnumerable<KeyValuePair<TKey, TValue>>? Value => this.ObservableCache.KeyValues;

    #endregion

    #endregion

    #region Get

    public override async ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>> Get(CancellationToken cancellationToken = default) 
        => GetResult<IEnumerable<KeyValuePair<TKey, TValue>>>.NoopSuccess(this.ObservableCache.KeyValues);

    #endregion

    #endregion
    
}
