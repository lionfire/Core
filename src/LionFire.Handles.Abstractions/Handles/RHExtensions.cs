using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    public static class RHExtensions
    {
        public static async Task<bool> TryEnsureRetrieved<T>(this RH<T> handle)
        {
            if (handle.HasObject) return true;

            if (handle.Reference == null) return false;

            return await handle.TryGetObject().ConfigureAwait(false);
        }

        public static bool HasReferenceOrObject<T>(this RH<T> handle) => handle != null && (handle.HasObject || handle.Reference != null);
    }
}
