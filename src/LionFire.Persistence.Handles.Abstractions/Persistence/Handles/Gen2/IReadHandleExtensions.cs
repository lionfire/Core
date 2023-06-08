using System;
using System.Threading.Tasks;
using LionFire.Persistence;
using LionFire.Data.Async.Gets;

namespace LionFire
{

    // REVIEW - if this is good, move it up out of the Gen2 directory

    public static class IReadHandleExtensions
    {
        //[Obsolete("Use TryResolveObject")]
        //public static async Task<bool> TryLoad(this IReadHandle<object> rh) 
        //{
        //    return await rh.Get().ConfigureAwait(false);
        //    //return await Task.Factory.StartNew(() =>
        //    //{
        //    //    return await rh.TryResolveObject();
        //    //    var _ = rh.Object;
        //    //    return true;
        //    //}).ConfigureAwait(false);
        //}

        public static (bool HasValue, T Value) GetValueWithoutRetrieve<T>(this IReadHandleBase<T> rh)
        {
            if (rh == null) return (false, default);
            if (rh.HasValue)
            {
                return (true, rh.Value);
            }
            else
            {
                return (false, default);
            }
        }

        public static bool IsWritable<T>(this IReadHandleBase<T> readHandle) => readHandle as ISets != null;

        public static async Task<bool> IsValueAvailable<T>(this IReadHandleBase<T> readHandle)
        {
            if (readHandle is ILazilyResolves<T> lr && lr.HasValue) return true;
            if (readHandle is ISupportsExist<T> ec) return await ec.Exists().ConfigureAwait(false);
            _ = await readHandle.Resolve().ConfigureAwait(false);
            return readHandle.HasValue;
        }
    }
}
