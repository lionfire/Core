using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Data.Gets;

public static class IGetsX
{
    /// <summary>
    /// (Uses reflection)
    /// </summary>
    /// <param name="gets"></param>
    /// <returns></returns>
    public static async Task<IGetResult<object>> GetUnknownType(this IStatelessGets gets, CancellationToken cancellationToken = default)
    {
        var getsInterface = gets.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IStatelessGets<>)).Single();
        return await ((ITask<IGetResult<object>>)getsInterface.GetMethod(nameof(IStatelessGets<object>.Get))!.Invoke(gets, new object?[] { cancellationToken })!).ConfigureAwait(false);
    }

    public static async Task<T> GetValue<T>(this IStatelessGets<T> resolves)
    {
        if (resolves is ILazilyGets<T> lazilyResolves)
        {
            return await lazilyResolves.GetIfNeeded<T>().ConfigureAwait(false);
        }

        var result = await resolves.Get().ConfigureAwait(false);
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
