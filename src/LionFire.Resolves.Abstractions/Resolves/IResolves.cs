using MorseCode.ITask;

namespace LionFire.Resolves
{
    public interface IResolves { }
    public interface IResolves<out TValue> : IResolves
    {
        /// <summary>
        /// Resolve the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyResolves to avoid unwanted re-resolving.)
        /// </summary>
        /// <returns></returns>
        ITask<IResolveResult<TValue>> Resolve();
    }

    //public interface IResolvesNongeneric // OLD
    //{
    //    /// <summary>
    //    /// Resolve the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyResolves to avoid unwanted re-resolving.)
    //    /// </summary>
    //    /// <returns></returns>
    //    Task<IResolveResult> Resolve();
    //}

    //public interface IResolvesConcrete<TValue>
    //{
    //    /// <summary>
    //    /// Resolve the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyResolves to avoid unwanted re-resolving.)
    //    /// </summary>
    //    /// <returns></returns>
    //    Task<IResolveResult<TValue>> Resolve();
    //}

    //public interface IResolvesCovariant<out TValue>
    //{
    //    /// <summary>
    //    /// Resolve the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyResolves to avoid unwanted re-resolving.)
    //    /// </summary>
    //    /// <returns></returns>
    //    Task<IResolveResult<object /* TValue */>> Resolve();
    //}
}
