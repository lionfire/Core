
namespace LionFire.Persistence;

public static class IReadHandleBaseX
{
    // OLD - Just Use ILazyRetrievable.Get instead?
    ///// <summary>
    ///// If HasObject is true, do nothing (regardless of whether it was retrieved, or set by the user).  Otherwise, retrieve the Object.
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="handle"></param>
    ///// <returns>handle.HasObject</returns>
    //public static async Task<bool> TryEnsureRetrieved<T>(this RH<T> handle)
    //{
    //    if (handle.HasObject) return true;

    //    if (handle.Reference == null) return false;

    //    await handle.Get<T>().ConfigureAwait(false);

    //    return handle.HasObject;
    //}

        // OLD - Just use .Object
//        public static async Task<T> ToObject<T>(this RH<T> handle)
//        {
//            if (handle.HasObject) return handle.Object;

//#if DEBUG
//            if (handle.Reference == null) throw new InvalidOperationException("handle.Reference should never be null");
//#endif

//            if (!(await handle.TryEnsureRetrieved().ConfigureAwait(false))) throw new NotFoundException();

//            return handle.Object;
//        }

    public static bool HasReferenceOrValue<TValue>(this IReadHandleBase<TValue> handle) => handle != null && (handle.Reference != null || (handle as ILazilyGets)?.HasValue == true);

    public static async Task<T> GetValue<T>(this IReadHandleBase<T> handle)
    {
        if (handle is ILazilyGets<T> l)
            return await l.GetIfNeeded<T>().ConfigureAwait(false);

        var result = await handle.Get().ConfigureAwait(false);
        if (!result.HasValue) throw new Exception("Failed to resolve value.");
        return result.Value;
    }

    public static async Task<IGetResult<T>> GetIfNeeded<T>(this IReadHandleBase<T> handle)
    {
        if (handle is ILazilyGets<T> l)
            return await l.GetIfNeeded().ConfigureAwait(false);

        var result = await handle.Get().ConfigureAwait(false);
        return result;
    }
}
