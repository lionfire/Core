using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Assets;
using LionFire.Handles;
using LionFire.Referencing;
using LionFire.Persistence;

namespace LionFire.Persistence.Assets
{

    //public class AssetResolver : IReferenceRetriever
    //{
    //    //public Task<RetrieveReferenceResult<T>> Retrieve<T>(IReadHandle<T> handle) where T : class
    //    //{
    //    //    throw new NotImplementedException();
    //    //}

    //    public Task<RetrieveResult<T>> Retrieve<T>(IReference reference) where T : class => throw new NotImplementedException();
    //}

    // Reads via Injection.GetService<IAssetProvider>
    // OPTIMIZE: Change the base class to an H base class that stores a string (key) instead of a IReference
    public class AssetReadHandle<T> : RBase<T>
        where T : class
    {

        #region Construction

        public AssetReadHandle() { }
        public AssetReadHandle(string assetSubPath)
        {
            this.Reference = new AssetReference<T>(assetSubPath);
        }
        
        #endregion

        //protected override IReference GetReferenceFromKey(string key)
        //{
        //    throw new NotImplementedException();
        //}

        //protected override string SetKeyFromReference(IReference reference)
        //{
        //    throw new NotImplementedException();
        //}

        public static implicit operator AssetReadHandle<T>(string assetSubPath)
        {
            return new AssetReadHandle<T>(assetSubPath);
        }

        public override Task<bool> TryRetrieveObject()
        {
            // TODO: Use async/await here once IAssetProvider supports it
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
