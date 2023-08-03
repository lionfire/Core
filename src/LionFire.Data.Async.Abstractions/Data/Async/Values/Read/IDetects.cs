
namespace LionFire.Data.Async.Gets;

public interface IDetects
{
    /// <summary>
    /// Retrieve from the source whether the object exists.
    /// 
    /// This always requests from the source.  See ILazilyDetects for a version that may return a cached value.
    /// 
    /// For ReadWrite objects, a StagedValue may be set locally, while the source has no Value.  In this case, you may wish to instead check a HasValue property.
    /// </summary>
    /// <seealso cref="TryGetObject"/>
    /// <returns>True if an object was found after a retrieval, false otherwise.</returns>
    Task<bool> Exists(); // REVIEW - pass to ICanResolveAsync, like with ResolveAsync?    
}

public interface IDetects<T> : IStatelessGetter<T>
{
    /// <summary>
    /// Check whether the Value exists at source.  In some cases, it is cheap to acquire the value.  In such cases, return the Value along with a positive Exists result.  If this returns Exists but no value, you must call Get to retrieve the Value.
    /// </summary>
    /// <param name="allowRememberValue">Only applies to ILazilyGets objects, which cache the value</param>
    /// <returns></returns>
    Task<IGetResult<T>> ExistsWithValue(bool allowRememberValue = true);
}

public static class IDetectsFallback
{

    //public static async Task<bool> Exists<T>(this IRetrieves<T> retrieves, bool forceCheck = false)
    //{
    //    throw new NotImplementedException("This needs more thought and logic");
    //    //if(retrieves is IDetects detects) { return await detects.Exists(forceCheck).ConfigureAwait(false);  }
    //    //if(retrieves is IHasPersistenceState hps)
    //    //{
    //    //    if (hps.NotFound()) return false;
    //    //    if (hps.State.HasFlag(TransferResultFlags.Found)) return true;
    //    //}
    //}
}
