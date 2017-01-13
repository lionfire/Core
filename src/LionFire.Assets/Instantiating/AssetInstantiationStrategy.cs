using LionFire.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public class AssetInstantiationStrategy : IInstantiationProvider
    {
        public static void Enable(InstantiationStrategyContext context = null)
        {
            (context ?? InstantiationStrategyContext.Default).Strategies.Add(30, new AssetInstantiationStrategy());
        }

        public IInstantiator TryProvide(object instance, InstantiationContext context = null)
        {
            var asset = instance as IAsset;
            if (asset == null || String.IsNullOrEmpty(asset.AssetSubPath)) return null;

            var ai = new AssetInstantiator()
            {
                TypeName = instance.GetType().FullName,
                AssetSubPath = asset.AssetSubPath,
            };

            context.Saving.RequiresSave.Add(instance);

            return ai;
        }
    }

    //public static class AssetObjectInstantiationExtensions
    //{
    //    //public
    //    //public static IAppHost
    //}

}
