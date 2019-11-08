﻿using System;
using LionFire.Persistence;
using LionFire.Resolves;

namespace LionFire.Persistence
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

        public static bool IsWritable<T>(this IReadHandleBase<T> readHandle) => readHandle as IPuts != null;

    }

}
