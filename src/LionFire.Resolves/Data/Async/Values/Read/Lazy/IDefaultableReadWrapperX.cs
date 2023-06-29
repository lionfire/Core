using LionFire.Data;
using LionFire.Data.Gets;
using LionFire.Structures;
using MorseCode.ITask;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods.Resolves;

public static class IDefaultableReadWrapperX
{
    #region Get

#if false // Can these upcasts be avoided?
    public static ITask<IGetResult<TValue>> GetValue<TValue>(this IDefaultableReadWrapper<TValue> readWrapper)
    {
        //if (readWrapper == null) return Task.FromResult((IGetResult<TValue>)NoopFailResolveResult<TValue>.Instance).AsITask();
        if (readWrapper is ILazilyGets<TValue> lr) return lr.TryGetValue(); // UPCAST
        return DefaultableReadWrapper_GetValue(readWrapper);
    }
#endif

    // REVIEW TODO - This is not a lazy resolve, but it returns LazyResolveResult which is misleading.
    internal static ITask<IGetResult<TValue>> DefaultableReadWrapper_GetValue<TValue>(IDefaultableReadWrapper<TValue> readWrapper)
    {
        var value = readWrapper.Value;
        return Task.FromResult((IGetResult<TValue>)new LazyResolveResult<TValue>(!EqualityComparer<TValue>.Default.Equals(value, default), value)).AsITask();
    }

    #endregion

    #region Query
#if false // Can these upcasts be avoided?
    public static IGetResult<TValue> QueryValue<TValue>(this IDefaultableReadWrapper<TValue> readWrapper)
    {
        if (readWrapper is ILazilyGets<TValue> lr) return lr.QueryValue<TValue>();

        // Only try accessing the Value if HasValue is true
        var hasValue = readWrapper.HasValue;
        return new LazyResolveResult<TValue>(hasValue, hasValue ? readWrapper.Value : default);
    }
#endif
    #endregion

}