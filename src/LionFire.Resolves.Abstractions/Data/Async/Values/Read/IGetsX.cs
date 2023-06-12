using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets;

public static class IGetsX
{
    /// <summary>
    /// (Uses reflection)
    /// </summary>
    /// <param name="resolves"></param>
    /// <returns></returns>
    public static async Task<IGetResult<object>> Resolve(this IGets resolves)
    {
        var retrievesInterface = resolves.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IGets<>)).Single();
        return await ((ITask<IGetResult<object>>)retrievesInterface.GetMethod(nameof(IGets<object>.Get)).Invoke(resolves, null)).ConfigureAwait(false);
    }

    public static async Task<T> GetValue<T>(this IGets<T> resolves)
    {
        if (resolves is ILazilyGets<T> lazilyResolves)
        {
            return await lazilyResolves.GetValue<T>().ConfigureAwait(false);
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
