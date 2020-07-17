using MorseCode.ITask;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Resolves
{
    public static class ILazilyResolvesExtensions
    {
        #region Generic

        public static async Task EnsureHasValue<T>(this ILazilyResolves<T> lazilyResolves)
        {
            if (!(await lazilyResolves.GetValue().ConfigureAwait(false)).HasValue) throw new Exception("EnsureHasValue: could not get value");
        }
        public static async Task<bool> TryEnsureHasValue<T>(this ILazilyResolves<T> lazilyResolves) 
            => !(await lazilyResolves.GetValue().ConfigureAwait(false)).HasValue;

        #endregion

        #region Non-generic

        public static Type GetLazilyResolvesType(this ILazilyResolves lazilyResolves)
        {
            var genericInterface = lazilyResolves.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ILazilyResolves<>)).Single();
            return genericInterface.GetGenericArguments()[0];
        }

        public static async ITask<ILazyResolveResult<object>> GetValue(this ILazilyResolves lazilyResolves)
        {
            var genericInterface = lazilyResolves.GetLazilyResolvesType();
            return (await ((ITask<ILazyResolveResult<object>>)
                genericInterface.GetMethod(nameof(ILazilyResolves<object>.GetValue)).Invoke(lazilyResolves, null)).ConfigureAwait(false));
        }

        public static async ITask<ILazyResolveResult<object>> QueryValue(this ILazilyResolves lazilyResolves)
        {
            var genericInterface = lazilyResolves.GetLazilyResolvesType();
            return (await ((ITask<ILazyResolveResult<object>>)
                genericInterface.GetMethod(nameof(ILazilyResolves<object>.QueryValue)).Invoke(lazilyResolves, null)).ConfigureAwait(false));
        }

        #endregion
    }



}
