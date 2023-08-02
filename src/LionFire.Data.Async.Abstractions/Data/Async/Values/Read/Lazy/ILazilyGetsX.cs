using MorseCode.ITask;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Data.Gets;

public static class ILazilyGetsX
{
    #region Generic

    public static async Task EnsureHasValue<T>(this IGets<T> lazilyResolves)
    {
        if (!(await lazilyResolves.GetIfNeeded().ConfigureAwait(false)).HasValue) throw new Exception("EnsureHasValue: could not get value");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="lazilyResolves"></param>
    /// <returns>HasValue</returns>
    public static async Task<bool> TryEnsureHasValue<T>(this IGets<T> lazilyResolves)
        => (await lazilyResolves.GetIfNeeded().ConfigureAwait(false)).HasValue;

    public static async Task<T> GetIfNeeded<T>(this IGets<T> lazilyResolves)
    {
        var result = await lazilyResolves.GetIfNeeded().ConfigureAwait(false);
        if (result.IsSuccess == false) throw result.ToException();
        if (!result.HasValue)
        {
            throw new ResolveException(result, "Failed to resolve value.");
        }
        return result.Value;
    }

    public static async Task<T> GetNonDefaultValue<T>(this IGets<T> lazilyResolves)
        where T : class
    {
        var result = await lazilyResolves.GetIfNeeded().ConfigureAwait(false);
        if (result.Value == default(T)) throw new Exception("Failed to resolve non-default value.");
        return result.Value;
    }

    public static T QueryNonDefaultValue<T>(this IGets<T> lazilyResolves)
        where T : class
    {
        var result =  lazilyResolves.QueryValue().Value;
        if (result == default(T)) throw new Exception("Failed to query non-default value.");
        return result;
    }

    public static async Task<T> GetValueOrDefault<T>(this IGets<T> lazilyResolves)
    {
        var result = await lazilyResolves.GetIfNeeded().ConfigureAwait(false);
#if SanityChecks
        // Will be default if !result.HasValue
        //if (!result.HasValue) Assert.True(result.Value == default);
#endif
        return result.Value; 
    }
    #endregion

    #region Non-generic

    public static Type GetLazilyGetsValueType(this ILazilyGets lazilyResolves)
    {
        var genericInterface = lazilyResolves.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IGets<>)).Single();
        return genericInterface;
        //return genericInterface.GetGenericArguments()[0];
    }

    // TODO: Split to OrDefault and NonDefaultValue like above?  Or just OrDefault and nothing.
    public static async ITask<ILazyGetResult<object>> GetValue(this ILazilyGets lazilyResolves)
    {
        var genericInterface = lazilyResolves.GetLazilyGetsValueType();
        return (await ((ITask<ILazyGetResult<object>>)
            genericInterface.GetMethod(nameof(IGets<object>.GetIfNeeded)).Invoke(lazilyResolves, null)).ConfigureAwait(false));
    }

    public static async ITask<ILazyGetResult<object>> QueryValue(this ILazilyGets lazilyResolves)
    {
        var genericInterface = lazilyResolves.GetLazilyGetsValueType();
        return (await ((ITask<ILazyGetResult<object>>)
            genericInterface.GetMethod(nameof(IGets<object>.QueryValue)).Invoke(lazilyResolves, null)).ConfigureAwait(false));
    }

    #endregion
}
