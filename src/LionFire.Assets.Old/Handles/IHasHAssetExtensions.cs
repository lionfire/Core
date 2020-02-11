#if TOPORT
using System;

namespace LionFire.Assets
{

    public static class IHasHAssetExtensions
    {
        //[Obsolete("Use HAsset.Save")]
        public static AssetType Save<AssetType>(this AssetBase<AssetType> me)
            where AssetType : class
        {
            me.HAsset.Save();
            return me.AssetObject;
        }

        [AotReplacement]
        public static object Save(this IHasHAsset me, Type concreteType)
        {
            me.HAsset.Save();
            return me.AssetObject;
        }
        public static object Save(this IHasHAsset me)
        {
            me.HAsset.Save();
            return me.AssetObject;
        }

    }

}
#endif