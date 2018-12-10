using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    public static class RExtensions
    {
        public static bool TryEnsureRetrieved<T>(this RH<T> handle)
        {
            if (handle.HasObject) return true;

            if (handle.Reference == null) return false;

            return handle.TryGetObject().Result;
        }
    }
}
