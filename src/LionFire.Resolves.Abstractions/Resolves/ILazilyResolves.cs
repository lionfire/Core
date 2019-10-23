using LionFire.Structures;
using System.Threading.Tasks;

namespace LionFire.Resolves
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue">If resolving to the default value (such as null) is possible, use a type wrapped with DefaultableValue&lt;T%gt; for TValue</typeparam>
    public interface ILazilyResolves<TValue> : ILazilyResolves, IResolves<TValue>, IReadWrapper<TValue>
    {
        Task<ILazyResolveResult<TValue>> GetValue();

        //new IHasDefaultableValueResult<TValue> HasValue { get; }
    }

    //public interface INotifyingLazilyResolves // Use Persistence instead?
    //{
    //    public event Action<ILazilyResolves> Resolved;
    //    public event Action<ILazilyResolves> Discarded;
    //}
}
