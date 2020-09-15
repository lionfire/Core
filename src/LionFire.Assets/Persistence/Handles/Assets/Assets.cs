using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Assets
{
    /// <summary>
    /// Convenience accessors
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Assets<T>
    {
        public static HC<T> HC => new AssetReference<T>().GetCollectionHandle<T>();
        public static IReadHandle<Metadata<IEnumerable<Listing<T>>>> List => new AssetReference<T>().GetListingsHandle<T>();

        public static IReadHandle<T> R(string assetPath) => new AssetReference<T>(assetPath).GetReadHandle<T, IAssetReference>();
        public static IReadWriteHandle<T> H(string assetPath) => new AssetReference<T>(assetPath).GetReadWriteHandle<T, IAssetReference>();
        //public static IReadWriteHandle<T> W(string assetPath) => new AssetReference<T>(assetPath).GetWriteHandle<T, IAssetReference>(); // FUTURE
    }
}
