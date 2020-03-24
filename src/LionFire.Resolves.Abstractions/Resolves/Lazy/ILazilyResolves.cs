using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Resolves
{
    public interface ILazilyResolves<out T> : ILazilyResolves, IDefaultableReadWrapper<T>
    {
        ITask<ILazyResolveResult<T>> GetValue();

        ILazyResolveResult<T> QueryValue();
    }

    public static class ILazilyResolvesExtensions
    {
        public static async Task EnsureHasValue<T>(this ILazilyResolves<T> lazilyResolves)
        {
            if (!(await lazilyResolves.GetValue().ConfigureAwait(false)).HasValue) throw new Exception("EnsureHasValue: could not get value");
        }
        public static async Task<bool> TryEnsureHasValue<T>(this ILazilyResolves<T> lazilyResolves) 
            => !(await lazilyResolves.GetValue().ConfigureAwait(false)).HasValue;
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
