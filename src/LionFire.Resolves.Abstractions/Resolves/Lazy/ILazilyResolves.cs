using LionFire.Structures;
using MorseCode.ITask;

namespace LionFire.Resolves
{
    public interface ILazilyResolves<out T> : ILazilyResolves, IDefaultableReadWrapper<T>
    {
        ITask<ILazyResolveResult<T>> GetValue();

        ILazyResolveResult<T> QueryValue();
    }


#if UNUSED

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue">If resolving to the default value (such as null) is possible, use a type wrapped with DefaultableValue&lt;T%gt; for TValue</typeparam>
    public interface ILazilyResolvesInvariant<TValue> : ILazilyResolves, IResolves<TValue>, IReadWrapper<TValue>
    {
        ITask<ILazyResolveResult<TValue>> GetValue();
    }

    public interface ILazilyResolvesConcrete<TValue> : ILazilyResolves, IResolves<TValue>, IReadWrapper<TValue>
    {
        Task<ILazyResolveResult<TValue>> GetValue();
    }
#endif
}
