using LionFire.Resolvers;
using MorseCode.ITask;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets;

/// <summary>
/// Marker interface indicating that a IGets&lt;TValue&gt; is likely also present.
/// </summary>
public interface IGets { }



/// <summary>
/// Resolve the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyResolves to avoid unwanted re-resolving.)
/// </summary>
public interface IGets<out TValue> : IGets
{
    /// <summary>
    /// Resolve the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyResolves to avoid unwanted re-resolving.)
    /// </summary>
    /// <returns></returns>
    ITask<IGetResult<TValue>> Resolve(); // TODO: CancelationToken and Timeout?
}


// For Orleans - REVIEW - is this really needed? It only strips out the covariance of TValue
public interface IGetsG<TValue> : IGets
{
    /// <summary>
    /// Resolve the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyResolves to avoid unwanted re-resolving.)
    /// </summary>
    /// <returns></returns>
    Task<IGetResult<TValue>> Resolve(); // TODO: CancelationToken and Timeout?
}
