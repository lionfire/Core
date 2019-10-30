using System;
using System.Threading.Tasks;
using LionFire.Persistence;
using LionFire.Resolves;

namespace LionFire.Persistence
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

        public static (bool HasValue, T Value) GetValueWithoutRetrieve<T>(this RH<T> rh)
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

        /// <summary>
        /// Fallback to provide ILazilyResolves<T>.Get to RH<T>
        /// Also makes Object return value strongly typed for the covariant ILazilyResolves.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rh"></param>
        /// <returns></returns>
        public static async Task<(bool HasValue, T Value)> GetValue<T>(this RH<T> rh) // RENAME: TryResolveNonNull
        {
            if (rh == null) return (false, default);

            if (rh is ILazilyResolves<T> lr)
            {
                var result = await lr.GetValue();
                return (result.HasValue, result.Value);
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

        public static bool IsWritable<T>(this RH<T> readHandle) => readHandle as IPuts != null;
    }

}
