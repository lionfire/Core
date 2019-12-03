using LionFire.Resolves;
using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods.Resolves
{
    /// <summary>
    /// These works on primitive interfaces, but upcast to ILazilyResolves&lt;T&gt; if possible
    /// </summary>
    public static class ILazilyResolvesFallbackExtensions
    {
        //public static ITask<IResolveResult<TValue>> GetValue<TValue>(this IDefaultable _)
        //{
        //    throw new Exception("TEST - does this get reached?");
        //}

        #region Get

        public static ITask<IResolveResult<TValue>> GetValue<TValue>(this IDefaultableReadWrapper<TValue> readWrapper)
        {
            //if (readWrapper == null) return Task.FromResult((IResolveResult<TValue>)NoopFailResolveResult<TValue>.Instance).AsITask();
            if (readWrapper is ILazilyResolves<TValue> lr) return lr.GetValue();
            return DefaultableReadWrapper_GetValue(readWrapper);
        }

        private static ITask<IResolveResult<TValue>> DefaultableReadWrapper_GetValue<TValue>(IDefaultableReadWrapper<TValue> readWrapper)
        {
            var value = readWrapper.Value;
            return Task.FromResult((IResolveResult<TValue>)new ResolveResult<TValue>(value != default, value)).AsITask();
        }

        /// <summary>
        /// If ILazilyResolves is not implemented, assumes Value getter is not an expensive operation and completes synchronously.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="readWrapper"></param>
        /// <returns></returns>
        public static ITask<IResolveResult<TValue>> GetValue<TValue>(this IReadWrapper<TValue> readWrapper)
        {
            if (readWrapper is ILazilyResolves<TValue> lr) return lr.GetValue();
            if (readWrapper is IDefaultableReadWrapper<TValue> wrapper) return DefaultableReadWrapper_GetValue(wrapper);

            // Otherwise, assume it doesn't lazily load.

            var value = readWrapper.Value;
            return Task.FromResult((IResolveResult<TValue>)new ResolveResult<TValue>(value != default, value)).AsITask();
        }

        /// <summary>
        /// If ILazilyResolves is not implemented, assumes Value getter is an expensive operation and completes asynchronously.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="readWrapper"></param>
        /// <returns></returns>
        public static ITask<IResolveResult<TValue>> GetValueExpensive<TValue>(this IReadWrapper<TValue> readWrapper)
        {
            if (readWrapper is ILazilyResolves<TValue> lr) return lr.GetValue();
            if (readWrapper is IDefaultableReadWrapper<TValue> wrapper) return DefaultableReadWrapper_GetValue(wrapper);

            // Otherwise, assume it lazily loads even without the ILazilyResolves<> interface.

            return Task.Run(() =>
            {
                var value = readWrapper.Value;
                return (IResolveResult<TValue>)new ResolveResult<TValue>(value != default, value);
            }).AsITask();
        }

        #endregion

        #region Query

        public static IResolveResult<TValue> QueryValue<TValue>(this IDefaultableReadWrapper<TValue> readWrapper)
        {
            if (readWrapper is ILazilyResolves<TValue> lr) return lr.QueryValue<TValue>();

            // Only try accessing the Value if HasValue is true
            var hasValue = readWrapper.HasValue;
            return new ResolveResult<TValue>(hasValue, hasValue ? readWrapper.Value : default);
        }

        /// <summary>
        /// If ILazilyResolves is not implemented, assumes Value getter is not an expensive operation.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="readWrapper"></param>
        /// <returns></returns>
        public static IResolveResult<TValue> QueryValue<TValue>(this IReadWrapper<TValue> readWrapper)
        {
            if (readWrapper is ILazilyResolves<TValue> lr) return lr.QueryValue<TValue>();

            var value = readWrapper.Value;
            return new ResolveResult<TValue>(value != default, value);
        }

        #endregion
    }
}
