using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Assets
{
    public static class IReadHandleExtensions
    {
        public static string AssetPath<T>(this IReadHandleBase<T> handle)
        {
            if (handle.Reference is IAssetReference ar) return ar.Path;
            if (handle.Value is IAssetPathAware apa) return apa.AssetPath;
            return null;
        }
    }
}
