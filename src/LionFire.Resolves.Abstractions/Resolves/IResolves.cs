using System.Threading.Tasks;

namespace LionFire.Resolves
{
    public interface IResolves
    {
        /// <summary>
        /// Resolve the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyResolves to avoid unwanted re-resolving.)
        /// </summary>
        /// <returns></returns>
        Task<IResolveResult> Resolve();
    }

    public interface IResolves<TValue>
    {
        Task<IResolveResult<TValue>> Resolve();
    }
}
