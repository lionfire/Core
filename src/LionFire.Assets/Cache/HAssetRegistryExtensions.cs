using System;

namespace LionFire.Assets
{
    public static class HAssetRegistryExtensions
    {
        public static HAsset<AssetType> ToHAsset<AssetType>(this string assetTypePath, AssetType obj = null)
            where AssetType : class => new AssetIdentifier<AssetType>(assetTypePath).ToHAsset();

        public static HAsset<AssetType> ToHAsset<AssetType>(this AssetIdentifier<AssetType> reference, AssetType obj = null)
            where AssetType : class
        {
            var ha = HAssetRegistry<AssetType>.Registry.GetOrAdd(reference, r => new HAsset<AssetType>(r) { IsRegistered = true });

            if (obj != null)
            {
                ha.Value = obj;
            }

            return ha;
        }

        public static HAsset<AssetType> ToRegistered<AssetType>(this HAsset<AssetType> hAsset, bool allowDiscardHAssetObject = true)
            where AssetType : class
        {
            if (hAsset == null) return null;
            if (hAsset.IsRegistered) return hAsset;
            if (hAsset.AssetTypePath == null) return hAsset; // Failed due to no path

            var registered = new AssetIdentifier<AssetType>(hAsset.AssetTypePath).ToHAsset();


            if (registered.HasValue && hAsset.HasValue)
            {
                if (!allowDiscardHAssetObject)
                {
                    if (!object.ReferenceEquals(registered.Value, hAsset.Value))
                    {
                        if (registered.Value != hAsset.Value)
                        {
                            throw new ArgumentException("registered.Object != hAsset.Object -- ToRegistered may lose data");
                        }
                    }
                }
            }
            else {
                if (!registered.HasValue && hAsset.HasValue)
                {
                    registered.Value = hAsset.Value;
                }
            }
            return registered;
        }

    }
}
