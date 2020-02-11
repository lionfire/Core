using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Assets
{
    public static class HAssetExtensions
    {
        public static HAsset<AssetType> ToHAsset<AssetType>(this string assetTypePath)
            where AssetType : class
            => new HAsset<AssetType>(assetTypePath);
    }
}
