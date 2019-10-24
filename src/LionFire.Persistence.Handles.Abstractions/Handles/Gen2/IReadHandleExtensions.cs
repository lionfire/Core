using System;
using System.Threading.Tasks;
using LionFire.Persistence;
using LionFire.Resolves;

namespace LionFire.Referencing
{

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

        /// <summary>
        /// Fallback to provide ILazilyResolves<TValue>.Get to RH<TValue>
        /// Also makes Object return value strongly typed for the covariant ILazilyResolves.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rh"></param>
        /// <returns></returns>
        public static async Task<(bool HasObject, T Object)> GetValue<T>(this RH<T> rh) // RENAME: TryResolveNonNull
        {
            if (rh is ILazilyResolves<T> lr)
            {
                var result = await lr.GetValue();
                return (result.HasValue, result.Value);
            }
            else if (rh is ILazilyResolvesCovariant<T> lrc)
            {
                var result = await lrc.GetValue();
                return (result.HasValue, (T)result.Value); // CAST
            }
            else
            {
                var obj = rh.Value;
                return (obj != default, obj);
            }
            //return (await rh.TryResolveObject().ConfigureAwait(false)) && rh.HasObject;  OLD
            //return await Task.Factory.StartNew(() =>
            //{
            //    var _ = rh.Object;
            //    return _ != null;
            //}).ConfigureAwait(false);
        }

        public static bool IsWritable<T>(this RH<T> readHandle) => readHandle as ICommitable != null;
    }

}
