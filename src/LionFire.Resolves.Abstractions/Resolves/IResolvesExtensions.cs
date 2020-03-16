using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Resolves
{
    public static class IResolvesExtensions
    {
        /// <summary>
        /// (Uses reflection)
        /// </summary>
        /// <param name="resolves"></param>
        /// <returns></returns>
        public static async Task<IResolveResult<object>> Resolve(this IResolves resolves)
        {
            var retrievesInterface = resolves.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IResolves<>)).Single();
            return await ((ITask<IResolveResult<object>>)retrievesInterface.GetMethod(nameof(IResolves<object>.Resolve)).Invoke(resolves, null)).ConfigureAwait(false);
        }
    }
}
