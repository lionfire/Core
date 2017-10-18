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
            base.Object = AssetSubPath.Load<T>(persistenceContext);
            return Task.FromResult(HasObject);
        }


        public static IEnumerable<AssetReadHandle<T>> All()
        {
            IAssetProvider AssetProvider = InjectionContext.Current.GetService<IAssetProvider>();
            foreach (var assetSubPath in AssetProvider.Find<T>())
            {
                yield return assetSubPath;
            }
        }
    }

    //public AssetReadHandle<TAsset> Clone()
    //{
    //    return new AssetReadHandle<TAsset>(assetSubPath);
    //}

}
