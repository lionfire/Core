using LionFire.Data.Async.Gets;
using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods.Resolves;

/// <summary>
/// These works on primitive interfaces, but upcast to ILazilyGets&lt;T&gt; if possible
/// </summary>
public static class ILazilyGetsFallbackExtensions
{
    //public static ITask<IGetResult<TValue>> GetValue<TValue>(this IDefaultable _)
    //{
    //    throw new Exception("TEST - does this get reached?");
    //}

    #region Get
        
    /// <summary>
    /// If ILazilyGets is not implemented, assumes Value getter is not an expensive operation and completes synchronously.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="readWrapper"></param>
    /// <returns></returns>
    public static ITask<IGetResult<TValue>> GetValue<TValue>(this IReadWrapper<TValue> readWrapper)
    {
        if (readWrapper is ILazilyGets<TValue> lr) return lr.TryGetValue();
        if (readWrapper is IDefaultableReadWrapper<TValue> wrapper) return IDefaultableReadWrapperX.DefaultableReadWrapper_GetValue(wrapper);

        // Otherwise, assume it doesn't lazily load.

        var value = readWrapper.Value;
        return Task.FromResult((IGetResult<TValue>)new LazyResolveResult<TValue>(!EqualityComparer<TValue>.Default.Equals(value, default), value)).AsITask();
    }

    /// <summary>
    /// If ILazilyGets is not implemented, assumes Value getter is an expensive operation and completes asynchronously.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="readWrapper"></param>
    /// <returns></returns>
    public static ITask<IGetResult<TValue>> GetValueExpensive<TValue>(this IReadWrapper<TValue> readWrapper)
    {
        if (readWrapper is ILazilyGets<TValue> lr) return lr.TryGetValue();
        if (readWrapper is IDefaultableReadWrapper<TValue> wrapper) return IDefaultableReadWrapperX.DefaultableReadWrapper_GetValue(wrapper);

        // Otherwise, assume it lazily loads even without the ILazilyGets<> interface.

        return Task.Run(() =>
        {
            var value = readWrapper.Value;
            return (IGetResult<TValue>)new LazyResolveResult<TValue>(!EqualityComparer<TValue>.Default.Equals(value, default), value);
        }).AsITask();
    }

    #endregion

    #region Query
       

    /// <summary>
    /// If ILazilyGets is not implemented, assumes Value getter is not an expensive operation.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="readWrapper"></param>
    /// <returns></returns>
    public static IGetResult<TValue> QueryValue<TValue>(this IReadWrapper<TValue> readWrapper)
    {
        if (readWrapper is ILazilyGets<TValue> lr) return lr.QueryValue<TValue>();

        var value = readWrapper.Value;
        return new LazyResolveResult<TValue>(!EqualityComparer<TValue>.Default.Equals(value, default), value);
    }

    #endregion
}
