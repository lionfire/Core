
namespace LionFire.Data.Async.Gets;

public interface IGetter<out TValue>  
    : IStatelessGetter<TValue>
    , ILazyGetter
    , IDefaultableReadWrapper<TValue>
    //, IObservableGetResults<TValue> // TODO: derive IObservableGetResults from IObservableGetOperations and eliminate this interface
    , IObservableGetOperations<TValue>
{
    ITask<IGetResult<TValue>> GetIfNeeded(); // TODO: Add CancellationToken

    /// <summary>
    /// REVIEW - change return type to T?  And separate this into LastResolveResult
    /// </summary>
    /// <returns></returns>
    IGetResult<TValue> QueryValue(); // RENAME to LatestGetResult, or GetResult, make a get property
    //IGetResult<T> GetResult { get; } // TODO
    
    TValue? ReadCacheValue { get; }

    
}

public interface ILazilyGetsValue<T> : IGetter<T>
{
    ValueTask<T> GetValueIfNeeded();
}

#if UNUSED

/// <summary>
/// 
/// </summary>
/// <typeparam name="TValue">If resolving to the default value (such as null) is possible, use a type wrapped with DefaultableValue&lt;T%gt; for TValue</typeparam>
public interface ILazilyResolvesInvariant<TValue> : ILazilyGets, IGets<TValue>, IReadWrapper<TValue>
{
    ITask<IGetResult<TValue>> GetValue();
}

public interface ILazilyResolvesConcrete<TValue> : ILazilyGets, IGets<TValue>, IReadWrapper<TValue>
{
    Task<IGetResult<TValue>> GetValue();
}
#endif
