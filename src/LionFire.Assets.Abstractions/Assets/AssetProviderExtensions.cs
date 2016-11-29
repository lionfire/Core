using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Structures;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    public static class AssetProviderExtensions
    {
        public static T Load<T>(this string assetSubPath) // TODO: Async?
        {
            var ap = (IAssetProvider)ManualSingleton<IServiceProvider>.Instance.GetService(typeof(IAssetProvider));
            return ap.Load<T>(assetSubPath);
        }
        public static T Load<T>(this string assetSubPath, string concreteTypeName) // TODO: Async?
        {
            var ap = (IAssetProvider)ManualSingleton<IServiceProvider>.Instance.GetService(typeof(IAssetProvider));
            return ap.Load<T>(assetSubPath);
        }

        public static void Save<T>(this string assetSubPath, T obj)
        {
            var ap = (IAssetProvider)ManualSingleton<IServiceProvider>.Instance.GetService(typeof(IAssetProvider));
             ap.Save<T>(assetSubPath, obj);
        }
    }
}
