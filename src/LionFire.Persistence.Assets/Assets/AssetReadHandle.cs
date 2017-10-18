using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Assets;
using LionFire.Handles;

namespace LionFire.Persistence.Assets
{
    public class AssetReadHandle<T> : ReadHandleBase<T>
        where T : class
    {

        public AssetReadHandle(string assetSubPath)
        {
            this.Key = assetSubPath;
        }
        public static implicit operator AssetReadHandle<T>(string assetSubPath)
        {
            return new AssetReadHandle<T>(assetSubPath);
        }

        public override Task<bool> TryResolveObject(object persistenceContext = null)
        {
            var ap = Injection.GetService<IAssetProvider>(createIfMissing: true);
            var result = ap.Load<T>(this.Key);
            if (result != null)
            {
                this.Object = result;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
    }

    //public class AssetReadHandle<T> : IReadHandle<T>
    //{
    //    string assetSubPath;
    //    public AssetReadHandle(string assetSubPath)
    //    {
    //        this.assetSubPath = assetSubPath;
    //    }

    //    public T Object
    //    {
    //        get
    //        {
    //            if (obj == null)
    //            {
    //                obj = assetSubPath.Load<T>();
    //            }
    //            return obj;
    //        }
    //    }
    //    private T obj;
    //    public bool HasObject { get { return } }
    //}

    //public static class ReadHandleExtensions
    //{
    //    public static IReadHandle<T> Handle<T>(this string assetSubPath)
    //    {
    //        return new AssetReadHandle<T>(assetSubPath);
    //    }
    //}
}
