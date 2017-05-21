using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Composables;
using LionFire.Assets;
using LionFire.Applications.Hosting;

namespace LionFire.Assets
{
    public static class IComposableExtensions
    {
        public static T AddAsset<TAsset, T>(this T composable, string assetSubPath) // MOVE to Assets DLL? 
            where T : class, IComposable<T>
            where TAsset : class
        {
            return composable.Add(new AssetReadHandle<TAsset>(assetSubPath));
        }

        public static IAppHost AddAsset<TAsset>(this IAppHost composable, string assetSubPath) // MOVE to Assets DLL? 
            where TAsset : class
        {
            return composable.Add(new AssetReadHandle<TAsset>(assetSubPath));
        }
    }
}
