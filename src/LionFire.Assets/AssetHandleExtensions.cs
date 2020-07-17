using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Assets
{
    public static class AssetHandleExtensions
    {
        public static IReadWriteHandle<TAsset> GetReadWriteAssetHandle<TAsset>(this TAsset asset)
            where TAsset : IAsset<TAsset>
        {
            if (asset is IReadWriteHandleAware<TAsset> aware && aware.ReadWriteHandle != null) return aware.ReadWriteHandle;
            var handle = new RWAsset<TAsset>(asset);
            if (asset is IReadWriteHandleAware<TAsset> aware2) { aware2.ReadWriteHandle = handle; }
            return handle;
        }

        public static IReadHandle<TAsset> GetReadAssetHandle<TAsset>(this TAsset asset)
         where TAsset : IAsset<TAsset>
        {
            if (asset is IReadHandleAware<TAsset> aware && aware.ReadHandle != null) return aware.ReadHandle;
            var handle = new RWAsset<TAsset>(asset);
            if (asset is IReadHandleAware<TAsset> aware2) { aware2.ReadHandle = handle; }
            return handle;
        }
    }
}
