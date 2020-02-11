using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Assets;

namespace LionFire.Persistence.Assets
{
    public static class AssetReferenceExtensions
    {
#if !AOT
        public static AssetReference<T> ToAssetReference<T>(this string name)
            where T : class, IAsset => throw new NotImplementedException("TOPORT - AssetPaths");
            //AssetPaths.AssetPathFromAssetTypePath<T>(name);
#endif
    }

}