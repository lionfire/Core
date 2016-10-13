using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Structures;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    public static class AssetProviderExtensions
    {
        public static T Load<T>(this string assetSubPath)
        {
            var ap = (IAssetProvider)ManualSingleton<IServiceProvider>.Instance.GetService(typeof(IAssetProvider));
            return ap.Load<T>(assetSubPath);
        }
    }
}
