using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Dependencies;

namespace LionFire.Data.Collections;

/// <summary>
/// An IObservableCache&lt;TValue, TKey&gt; that is retrieved asynchronously.
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
    where TValue : notnull
{
    AsyncObservableCollectionOptions Options { get; }
    #region Lifecycle

    //public AsyncReadOnlyDictionary() : this(null) { }

    public AsyncReadOnlyDictionary(AsyncObservableCollectionOptions? options = null) 
    {
        Options = options ?? AsyncObservableCollectionOptions.Default;
        SourceCache = new SourceCache<KeyValuePair<TKey, TValue>, TKey>(tuple => tuple.Key);
    }

    #endregion

    #region State

    protected SourceCache<KeyValuePair<TKey, TValue>, TKey> SourceCache { get; }


    #region Methods

    public override void DiscardValue()
    {
        SourceCache.Clear();
        //hasValue = false;
    }

    #endregion

    #region Derived

    public IObservableCache<KeyValuePair<TKey, TValue>, TKey> ObservableCache => SourceCache.AsObservableCache(); // Converts to read only

    public override IEnumerable<KeyValuePair<TKey, TValue>>? ReadCacheValue
    {
        get => SourceCache.Items;
        //protected set
        //{
        //    SourceCache.EditDiff((value ?? Enumerable.Empty<KeyValuePair<TKey, TValue>>())
        //        .Select(kvp => kvp.Value),
        //        (v1, v2) => SourceCache.KeySelector(v1).Equals(SourceCache.KeySelector(v2)));
        //}
    }
    public override IEnumerable<KeyValuePair<TKey, TValue>>? Value => this.SourceCache.Items;

    #endregion

    #endregion

    public override void OnNext(IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>> result)
    {
        if (result.IsSuccess())
        {
            SourceCache.EditDiff(result.Value ?? Enumerable.Empty<KeyValuePair<TKey, TValue>>(),
                (v1, v2) => v1.Key.Equals(v2.Key));

//            SourceCache.EditDiff((result.Value ?? Enumerable.Empty<KeyValuePair<TKey, TValue>>())
//                   //.Select(kvp => kvp.Value),
//,                   (v1, v2) => v1.Key == v2.Key /* SourceCache.KeySelector(v1).Equals(SourceCache.KeySelector(v2))*/ );
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
