using System;
using System.Threading.Tasks;
using LionFire.Persistence;

namespace LionFire.Referencing
{

    public static class IReadHandleExtensions
    {
        [Obsolete("Use TryResolveObject")]
        public static async Task<bool> TryLoad(this IReadHandle<object> rh) 
        {
            return await rh.TryResolveObject().ConfigureAwait(false);
            //return await Task.Factory.StartNew(() =>
            //{
            //    return await rh.TryResolveObject();
            //    var _ = rh.Object;
            //    return true;
            //}).ConfigureAwait(false);
        }


        public static async Task<bool> TryLoadNonNull(this IReadHandle<object> rh) // RENAME: TryResolveNonNull
        {
            return (await rh.TryResolveObject().ConfigureAwait(false)) && rh.HasObject;
            //return await Task.Factory.StartNew(() =>
            //{
            //    var _ = rh.Object;
            //    return _ != null;
            //}).ConfigureAwait(false);
        }

        public static bool IsWritable<T>(this IReadHandle<T> readHandle)
        {
            return readHandle as ICommitable != null;
        }
    }

}
