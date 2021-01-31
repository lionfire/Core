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
            if (!(await lazilyResolves.TryGetValue().ConfigureAwait(false)).HasValue) throw new Exception("EnsureHasValue: could not get value");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lazilyResolves"></param>
        /// <returns>HasValue</returns>
        public static async Task<bool> TryEnsureHasValue<T>(this ILazilyResolves<T> lazilyResolves)
            => (await lazilyResolves.TryGetValue().ConfigureAwait(false)).HasValue;

        public static async Task<T> GetValue<T>(this ILazilyResolves<T> lazilyResolves)
        {
            var result = await lazilyResolves.TryGetValue().ConfigureAwait(false);
            if (!result.HasValue) throw new Exception("Failed to resolve value.");
            return result.Value;
        }

        public static async Task<T> GetNonDefaultValue<T>(this ILazilyResolves<T> lazilyResolves)
            where T : class
        {
            var result = await lazilyResolves.TryGetValue().ConfigureAwait(false);
            if (result.Value == default(T)) throw new Exception("Failed to resolve non-default value.");
            return result.Value;
        }

        public static T QueryNonDefaultValue<T>(this ILazilyResolves<T> lazilyResolves)
            where T : class
        {
            var result =  lazilyResolves.QueryValue().Value;
            if (result == default(T)) throw new Exception("Failed to query non-default value.");
            return result;
        }

        public static async Task<T> GetValueOrDefault<T>(this ILazilyResolves<T> lazilyResolves)
        {
            var result = await lazilyResolves.TryGetValue().ConfigureAwait(false);
#if SanityChecks
            // Will be default if !result.HasValue
            //if (!result.HasValue) Assert.True(result.Value == default);
#endif
            return result.Value; 
        }
        #endregion

        #region Non-generic

        public static Type GetLazilyResolvesType(this ILazilyResolves lazilyResolves)
        {
            var genericInterface = lazilyResolves.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ILazilyResolves<>)).Single();
            return genericInterface;
            //return genericInterface.GetGenericArguments()[0];
        }

        public static async ITask<ILazyResolveResult<object>> GetValue(this ILazilyResolves lazilyResolves)
        {
            var genericInterface = lazilyResolves.GetLazilyResolvesType();
            return (await ((ITask<ILazyResolveResult<object>>)
                genericInterface.GetMethod(nameof(ILazilyResolves<object>.TryGetValue)).Invoke(lazilyResolves, null)).ConfigureAwait(false));
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
