using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Resolves;

public static class IResolvesX
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

    public static async Task<T> GetValue<T>(this IResolves<T> resolves)
    {
        if (resolves is ILazilyResolves<T> lazilyResolves)
        {
            return await lazilyResolves.GetValue<T>().ConfigureAwait(false);
        }

        var result = await resolves.Resolve().ConfigureAwait(false);
        if (result.IsSuccess == true)
        {
            return result.Value;
        }
        else
        {
            throw result.ToException();
        }
    }
}
