using System;
using System.Threading.Tasks;
using LionFire.Persistence;

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
        /// Fallback to provide ILazilyRetrievable<T>.Get to RH<T>
        /// Also makes Object return value strongly typed for the covariant ILazilyRetrievable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rh"></param>
        /// <returns></returns>
        public static async Task<(bool HasObject, T Object)> Get<T>(this RH<T> rh) // RENAME: TryResolveNonNull
        {
            if (rh is ILazilyRetrievable<T> rhex)
            {
                var result = await rhex.Get();
                return (result.HasObject, (T)result.Object); // CAST
            }
            else
            {
                var obj = rh.Object;
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
