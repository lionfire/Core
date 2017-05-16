using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Composables;
using LionFire.Assets;

namespace LionFire.Assets
{
    public static class IComposableExtensions
    {
        public static T AddAsset<T>(this T composable, string assetSubPath) // MOVE to Assets DLL? after renaming IAppHost to IComposable and moving it to runtime? 
            where T : class, IComposable<T>
        {
            return composable.Add(new AssetReadHandle<T>(assetSubPath));
        }
    }
}
