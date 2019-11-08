using LionFire.Persistence.Handles;
using System;
using System.Collections.Generic;

namespace LionFire.Persistence
{
    public static class IReadWriteHandlePairExtensions
    {

        /// <summary>
        /// Compares LocalHandle.Value to RemoteHandle.Value.
        /// If either handle is missing, or has HasValue == false, -2 is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handle"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int Compare<T>(this IReadWriteHandlePair<T> handle, IComparer<T> comparer = null) {
            if (handle.WriteHandle == null || !handle.WriteHandle.HasValue) return -2;
            if (handle.ReadHandle == null || !handle.ReadHandle.HasValue) return -2;
            return (comparer ?? Comparer<T>.Default).Compare(handle.WriteHandle.Value, handle.ReadHandle.Value);
        }
        public static bool IsDeepCloneable<T>(this IReadWriteHandlePair<T> handle) { throw new NotImplementedException(); } // TODO
    }
}

