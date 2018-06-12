using LionFire.DependencyInjection;
using LionFire.Handles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    public class AssetReadHandle<T> : ReadHandleBase<T>, IReadHandle<T>
        where T : class
    {
        [SetOnce]
        public string AssetSubPath
        {
            get => Key; set => Key = value;
        }

        public AssetReadHandle(string assetSubPath)
        {
            this.AssetSubPath = assetSubPath;
        }

        public static implicit operator AssetReadHandle<T>(string assetSubPath)
        {
            return new AssetReadHandle<T>(assetSubPath);
        }

        public override Task<bool> TryResolveObject(object persistenceContext = null)
        {
            IAssetProvider AssetProvider = InjectionContext.Current.GetService<IAssetProvider>();
            base.Object = AssetProvider.Load<T>(AssetSubPath, persistenceContext);
            //base.Object = AssetSubPath.Load<T>(persistenceContext);
            return Task.FromResult(HasObject);
        }


        public static async Task<IEnumerable<AssetReadHandle<T>>> All()
        {
            IAssetProvider AssetProvider = InjectionContext.Current.GetService<IAssetProvider>();

            List<AssetReadHandle<T>> results = new List<AssetReadHandle<T>>();

            foreach (var assetSubPath in (await AssetProvider.Find<T>()))
            {
                results.Add(assetSubPath);
            }
            return results;
        }
    }

    //public AssetReadHandle<TAsset> Clone()
    //{
    //    return new AssetReadHandle<TAsset>(assetSubPath);
    //}

}
