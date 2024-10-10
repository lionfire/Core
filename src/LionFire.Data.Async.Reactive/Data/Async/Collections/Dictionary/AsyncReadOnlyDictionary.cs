using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Dependencies;

namespace LionFire.Data.Collections;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// Parameters:
///  - KeySelector (stored in SourceCache)
///  
/// State:
///  - SourceCache
///  - HasValue
///  
/// Implements:
///  - ObservableCache via SourceCache
///  - ReadCacheValue via SourceCache
///  - DiscardValue
///  
/// Abstract:
///  - IObservable<IGetResult<>> IObservableGetOperations.GetOperations
/// </remarks>
public abstract class AsyncReadOnlyDictionary<TKey, TValue>
    : AsyncDynamicDataCollection<KeyValuePair<TKey, TValue>>
    , IAsyncReadOnlyDictionary<TKey, TValue>
    //, IInjectable<IKeyProvider<TKey, TValue>> // REVIEW - this is messy. Is it needed?
    where TKey : notnull
{
    //#region Dependencies

    //#region Optional

    //#region KeyProvider

    // OLD: don't inject this. Rely on injected keySelector Func instead.
    //public IKeyProvider<TKey, TValue>? KeyProvider { get; set; }

    //IKeyProvider<TKey, TValue>? IHas<IKeyProvider<TKey, TValue>>.Object => KeyProvider;
    //IKeyProvider<TKey, TValue> IDependsOn<IKeyProvider<TKey, TValue>>.Dependency { set => KeyProvider = value; }

    //#endregion

    //#endregion

    //#endregion

    #region Parameters

    // TODO: Eliminate KeySelector for this class, since keys tend to come from user, not from TValue.  If keys come from TValue, use AsyncReadOnlyKeyedCollectionCache instead.
    protected Func<TValue, TKey> KeySelector => SourceCache.KeySelector;
    public virtual Func<TValue, TKey> DefaultKeySelector() => KeySelectors<TValue, TKey>.GetKeySelector(DependencyContext.Current?.ServiceProvider);

    #endregion

    #region Lifecycle

    public AsyncReadOnlyDictionary() : this(null) { }

    public AsyncReadOnlyDictionary(Func<TValue, TKey>? keySelector, SourceCache<TValue, TKey>? sourceCache = null, AsyncObservableCollectionOptions? options = null)
    {
        SourceCache = sourceCache ?? new SourceCache<TValue, TKey>(keySelector ?? DefaultKeySelector());

    }

    #endregion

    #region State

    protected SourceCache<TValue, TKey> SourceCache { get; }
    //public override bool HasValue => hasValue;
    //protected bool hasValue;

    #region Methods

    public override void DiscardValue()
    {
        SourceCache.Clear();
        //hasValue = false;
    }

    #endregion

    #region Derived

    public IObservableCache<TValue, TKey> ObservableCache => SourceCache.AsObservableCache(); // Converts to read only

    public override IEnumerable<KeyValuePair<TKey, TValue>>? ReadCacheValue
    {
        get => SourceCache.KeyValues;
        //protected set
        //{
        //    SourceCache.EditDiff((value ?? Enumerable.Empty<KeyValuePair<TKey, TValue>>())
        //        .Select(kvp => kvp.Value),
        //        (v1, v2) => SourceCache.KeySelector(v1).Equals(SourceCache.KeySelector(v2)));
        //}
    }
    public override IEnumerable<KeyValuePair<TKey, TValue>>? Value => this.SourceCache.KeyValues;

    #endregion

    #endregion

    public override void OnNext(IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>> result)
    {
        if (result.IsSuccess())
        {
            SourceCache.EditDiff((result.Value ?? Enumerable.Empty<KeyValuePair<TKey, TValue>>())
                   .Select(kvp => kvp.Value),
                   (v1, v2) => SourceCache.KeySelector(v1).Equals(SourceCache.KeySelector(v2)));
        }
    }

    #region (explicit) IAsyncCollection<TValue>

    // TODO: Several not implemented. Would benefit from being able to clone LazyResolveResult and other ResolveResults with a different type (overriding or transforming value)

    // Allows treating this as a list

    #endregion

    #region IAsyncReadOnlyCollectionCache<KeyValuePair<TKey, TItem>>

    // OLD - Redundant
    //#region IObservableGets<IEnumerable<TItem>>

    // IObservableGetOperations<IEnumerable<KeyValuePair<TKey, TValue>>>.
    //public abstract IObservable<ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>>> GetOperations { get; }

    //#endregion

    // OLD - Redundant?
    //IEnumerable<KeyValuePair<TKey, TValue>>? IReadWrapper<IEnumerable<KeyValuePair<TKey, TValue>>>.Value => this.Value;

    #region ILazilyGets<IEnumerable<TItem>>

    // OLD - Redundant
    //async ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>> IGetter<IEnumerable<KeyValuePair<TKey, TValue>>>.GetIfNeeded()
    //{
    //    var result = await this.GetIfNeeded().ConfigureAwait(false);
    //    throw new NotImplementedException();
    //}

    // OLD - Redundant
    //IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>> IGetter<IEnumerable<KeyValuePair<TKey, TValue>>>.QueryValue()
    //{
    //    var result = this.QueryValue();

    //    //if (result.IsSuccess) // THREADUNSAFE
    //    //{
    //    //    return new ResolveResultSuccess<IEnumerable<KeyValuePair<TKey, TValue>>>(SourceCache.KeyValues);
    //    //}
    //    throw new NotImplementedException();
    //}

    #endregion

    #region IGets<IEnumerable<KeyValuePair<TKey, TValue>>>

    // OLD - Redundant
    //async ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>> IStatelessGetter<IEnumerable<KeyValuePair<TKey, TValue>>>.Get(CancellationToken cancellationToken)
    //{
    //    await this.Get(cancellationToken).ConfigureAwait(false);
    //    throw new NotImplementedException();
    //}

    #endregion

    #endregion

    #region Get

    //public override async ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>> Get(CancellationToken cancellationToken = default)
    //{
    //    try
    //    {
    //        var resultTask = GetImpl(cancellationToken);
    //        //hasValue = true; // REVIEW TODO: if resultTask fails, it didn't really get the value
    //        getOperations.OnNext(resultTask);

    //        return await resultTask.ConfigureAwait(false);
    //    }
    //    catch (Exception ex)
    //    {
    //        return GetResult<IEnumerable<KeyValuePair<TKey, TValue>>>.Exception(ex);
    //    }
    //}

    //protected abstract ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>> GetImpl(CancellationToken cancellationToken = default);

    #endregion
}
