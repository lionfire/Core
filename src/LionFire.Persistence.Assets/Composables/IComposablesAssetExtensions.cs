using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Composables;
using LionFire.Assets;
using LionFire.Applications.Hosting;
using System.Linq;
using LionFire.Structures;

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


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="composable"></param>
        /// <param name="enabledOnly">Do not add Assets that implement IEnableable and have IEnableable.IsEnabled == false</param>
        /// <returns></returns>
        public static IAppHost AddAllAssets<TAsset>(this IAppHost composable, bool enabledOnly = true)
            where TAsset : class
        {
            // ENH: Hot initialize -- monitor assets folder for changes and add to AppHost during runtime, and the app will re-init itself based on some policy (maybe throttle 2sec)

            var childHandles = AssetReadHandle<TAsset>.All();
            if (enabledOnly)
            {
                childHandles = childHandles.Where(ch =>
                {
                    if (ch is IEnableable e)
                    {
                        if (e.IsEnabled == false)
                        {
                            ch.ForgetObject();
                            return false;
                        }
                    }
                    return true;
                });
            }
            foreach (var asset in childHandles)
            {
                composable.Add(asset.Object);
            }
            return composable;
        }
    }
}
