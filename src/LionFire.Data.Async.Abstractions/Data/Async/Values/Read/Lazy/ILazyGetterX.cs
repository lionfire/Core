using MorseCode.ITask;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets;

public static class ILazyGetterX
{
    #region Generic

    public static async Task EnsureHasValue<T>(this IGetter<T> lazilyResolves)
    {
        if (!(await lazilyResolves.GetIfNeeded().ConfigureAwait(false)).HasValue) throw new Exception("EnsureHasValue: could not get value");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="lazilyResolves"></param>
    /// <returns>HasValue</returns>
    public static async Task<bool> TryEnsureHasValue<T>(this IGetter<T> lazilyResolves)
        => (await lazilyResolves.GetIfNeeded().ConfigureAwait(false)).HasValue;

    public static async Task<T?> GetIfNeeded<T>(this IGetter<T> lazilyResolves)
    {
        var result = await lazilyResolves.GetIfNeeded().ConfigureAwait(false);
        if (result.IsSuccess == false) throw result.ToException();
        if (!result.HasValue)
        {
            throw new ResolveException(result, "Failed to resolve value.");
        }
        return result.Value;
    }

    public static async Task<T?> GetNonDefaultValue<T>(this IGetter<T> lazilyResolves)
        where T : class
    {
        var result = await lazilyResolves.GetIfNeeded().ConfigureAwait(false);
        if (result.Value == default(T)) throw new Exception("Failed to resolve non-default value.");
        return result.Value;
    }

    public static T QueryNonDefaultValue<T>(this IGetter<T> lazilyResolves)
        where T : class
    {
        var result =  lazilyResolves.QueryGetResult().Value;
        if (result == default(T)) throw new Exception("Failed to query non-default value.");
        return result;
    }

    public static async Task<T?> GetValueOrDefault<T>(this IGetter<T> lazilyResolves)
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

    public static Type GetLazilyGetsValueType(this ILazyGetter lazilyResolves)
    {
        var genericInterface = lazilyResolves.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IGetter<>)).Single();
        return genericInterface;
        //return genericInterface.GetGenericArguments()[0];
    }

    // TODO: Split to OrDefault and NonDefaultValue like above?  Or just OrDefault and nothing.
    public static async ITask<IGetResult<object>> GetValue(this ILazyGetter lazilyResolves)
    {
        var genericInterface = lazilyResolves.GetLazilyGetsValueType();
        return (await ((ITask<IGetResult<object>>)
            genericInterface.GetMethod(nameof(IGetter<object>.GetIfNeeded))!.Invoke(lazilyResolves, null)!).ConfigureAwait(false));
    }

    public static async ITask<IGetResult<object>> QueryValue(this ILazyGetter lazilyResolves)
    {
        var genericInterface = lazilyResolves.GetLazilyGetsValueType();
        return (await ((ITask<IGetResult<object>>)
            genericInterface.GetMethod(nameof(IGetter<object>.QueryGetResult))!.Invoke(lazilyResolves, null)!).ConfigureAwait(false));
    }

    #endregion
}
