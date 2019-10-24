using MorseCode.ITask;
using System.Threading.Tasks;

namespace LionFire.Resolves
{
    //public interface IResolves
    //{
    //    /// <summary>
    //    /// Resolve the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyResolves to avoid unwanted re-resolving.)
    //    /// </summary>
    //    /// <returns></returns>
    //    Task<IResolveResult> Resolve();
    //}

    public interface IResolvesConcrete<TValue>
    {
        /// <summary>
        /// Resolve the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyResolves to avoid unwanted re-resolving.)
        /// </summary>
        /// <returns></returns>
        Task<IResolveResult<TValue>> Resolve();
    }

    // TODO: If this works, get rid of IResolvesConcrete and IResolvesCovariant
    public interface IResolves<out TValue>
    {
        /// <summary>
        /// Resolve the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyResolves to avoid unwanted re-resolving.)
        /// </summary>
        /// <returns></returns>
        ITask<IResolveResult<TValue>> Resolve();
    }

    public interface IResolvesCovariant<out TValue>
    {
        /// <summary>
        /// Resolve the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyResolves to avoid unwanted re-resolving.)
        /// </summary>
        /// <returns></returns>
        Task<IResolveResult<object /* TValue */>> Resolve();
    }
}
