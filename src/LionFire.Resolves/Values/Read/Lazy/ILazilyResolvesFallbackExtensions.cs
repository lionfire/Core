using LionFire.Data.Async.Gets;
using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods.Resolves;

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
        
    /// <summary>
    /// If ILazilyResolves is not implemented, assumes Value getter is not an expensive operation and completes synchronously.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="readWrapper"></param>
    /// <returns></returns>
    public static ITask<IResolveResult<TValue>> GetValue<TValue>(this IReadWrapper<TValue> readWrapper)
    {
        if (readWrapper is ILazilyResolves<TValue> lr) return lr.TryGetValue();
        if (readWrapper is IDefaultableReadWrapper<TValue> wrapper) return IDefaultableReadWrapperX.DefaultableReadWrapper_GetValue(wrapper);

        // Otherwise, assume it doesn't lazily load.

        var value = readWrapper.Value;
        return Task.FromResult((IResolveResult<TValue>)new LazyResolveResult<TValue>(!EqualityComparer<TValue>.Default.Equals(value, default), value)).AsITask();
    }

    /// <summary>
    /// If ILazilyResolves is not implemented, assumes Value getter is an expensive operation and completes asynchronously.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="readWrapper"></param>
    /// <returns></returns>
    public static ITask<IResolveResult<TValue>> GetValueExpensive<TValue>(this IReadWrapper<TValue> readWrapper)
    {
        if (readWrapper is ILazilyResolves<TValue> lr) return lr.TryGetValue();
        if (readWrapper is IDefaultableReadWrapper<TValue> wrapper) return IDefaultableReadWrapperX.DefaultableReadWrapper_GetValue(wrapper);

        // Otherwise, assume it lazily loads even without the ILazilyResolves<> interface.

        return Task.Run(() =>
        {
            var value = readWrapper.Value;
            return (IResolveResult<TValue>)new LazyResolveResult<TValue>(!EqualityComparer<TValue>.Default.Equals(value, default), value);
        }).AsITask();
    }

    #endregion

    #region Query
       

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
        return new LazyResolveResult<TValue>(!EqualityComparer<TValue>.Default.Equals(value, default), value);
    }

    #endregion
}
