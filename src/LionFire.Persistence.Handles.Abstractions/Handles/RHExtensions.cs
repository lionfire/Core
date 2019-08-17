using System;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public static class RHExtensions
    {
        /// <summary>
        /// If HasObject is true, do nothing (regardless of whether it was retrieved, or set by the user).  Otherwise, retrieve the Object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handle"></param>
        /// <returns>handle.HasObject</returns>
        public static async Task<bool> TryEnsureRetrieved<T>(this RH<T> handle)
        {
            if (handle.HasObject) return true;

            if (handle.Reference == null) return false;

            await handle.Get<T>().ConfigureAwait(false);

            return handle.HasObject;
        }

        public static async Task<T> ToObject<T>(this RH<T> handle)
        {
            if (handle.HasObject) return handle.Object;

#if DEBUG
            if (handle.Reference == null) throw new InvalidOperationException("handle.Reference should never be null");
#endif

            if (!(await handle.TryEnsureRetrieved().ConfigureAwait(false))) throw new NotFoundException();

            return handle.Object;
        }

        public static bool HasReferenceOrObject<T>(this RH<T> handle) => handle != null && (handle.HasObject || handle.Reference != null);
    }
}
