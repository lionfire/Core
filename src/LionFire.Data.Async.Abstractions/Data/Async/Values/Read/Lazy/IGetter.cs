
namespace LionFire.Data.Async.Gets;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// Suggested additional interface: IConfigurableGetter
/// </remarks>
public interface IGetter<out TValue>  
    : IStatelessGetter<TValue>
    , ILazyGetter
    , IDefaultableReadWrapper<TValue>
    , IObservableGetOperations<TValue>
{
    ITask<IGetResult<TValue>> GetIfNeeded(); // TODO: Add CancellationToken

    /// <summary>
    /// Synchronously transforms any ReadCacheValue into a GetResult
    /// </summary>
    /// <returns></returns>
    IGetResult<TValue> QueryGetResult(); 

    /// ENH idea
    //IGetResult<TValue>? LastGetResult();

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
