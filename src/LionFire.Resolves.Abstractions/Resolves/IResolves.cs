using MorseCode.ITask;

namespace LionFire.Resolves
{
    /// <summary>
    /// Marker interface indicating that a IResolves&lt;TValue&gt; is likely also present.
    /// </summary>
    public interface IResolves { }

    /// <summary>
    /// Resolve the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyResolves to avoid unwanted re-resolving.)
    /// </summary>
    public interface IResolves<out TValue> : IResolves
    {
        /// <summary>
        /// Resolve the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyResolves to avoid unwanted re-resolving.)
        /// </summary>
        /// <returns></returns>
        ITask<IResolveResult<TValue>> Resolve();
    }
}
