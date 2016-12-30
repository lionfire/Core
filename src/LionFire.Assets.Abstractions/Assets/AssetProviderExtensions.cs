using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Structures;
using System.Threading.Tasks;

namespace LionFire.Assets
{

    public static class AssetProviderExtensions
    {
        public static IEnumerable<string> Find<T>(this string searchString)
        {
            var ap = (IAssetProvider)ManualSingleton<IServiceProvider>.Instance.GetService(typeof(IAssetProvider));
            return ap.Find<T>(searchString);
        }

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

        //public static void Save<T>(this string assetSubPath, T obj)
        //{
        //    var ap = (IAssetProvider)ManualSingleton<IServiceProvider>.Instance.GetService(typeof(IAssetProvider));
        //     ap.Save<T>(assetSubPath, obj);
        //}

        public static void Save<T>(this T obj, string assetSubPath)
        {
            var ap = (IAssetProvider)ManualSingleton<IServiceProvider>.Instance.GetService(typeof(IAssetProvider));
            ap.Save<T>(assetSubPath, obj);
        }

        

    }
}
