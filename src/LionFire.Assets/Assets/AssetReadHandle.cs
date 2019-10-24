using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Assets;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Persistence;

namespace LionFire.Persistence.Assets
{

    //public class AssetResolver : IReferenceRetriever
    //{
    //    //public Task<RetrieveReferenceResult<TValue>> Retrieve<TValue>(IReadHandle<TValue> handle) where TValue : class
    //    //{
    //    //    throw new NotImplementedException();
    //    //}

    //    public Task<RetrieveResult<TValue>> Retrieve<TValue>(IReference reference) where TValue : class => throw new NotImplementedException();
    //}

    // Reads via Injection.GetService<IAssetProvider>
    // OPTIMIZE: Change the base class to an H base class that stores a string (key) instead of a IReference
    public class AssetReadHandle<T> : RBaseEx<T>
        where T : class
    {

        #region Construction

        public AssetReadHandle() { }
        public AssetReadHandle(string assetSubPath)
        {
            this.Reference = new AssetReference<T>(assetSubPath);
        }

        public AssetReadHandle(IReference reference, T obj = null) : base(reference, obj)
        {
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

        
        public override Task<IRetrieveResult<T>> RetrieveImpl()
        {
            
            throw new NotImplementedException("TODO: Convert Assets to an OBus");
            //// TODO: Use async/await here once IAssetProvider supports it
            //var ap = Dependencies.GetServiceOrSingleton<IAssetProvider>(createIfMissing: true);
            //var result = await ap.Load<TValue>(this.Key);
            //if (result != null)
            //{
            //    this.Object = result;
            //    return Task.FromResult(true);
            //}
            //else
            //{
            //    return Task.FromResult(false);
            //}
        }
    }

    //public class AssetReadHandle<TValue> : IReadHandle<TValue>
    //{
    //    string assetSubPath;
    //    public AssetReadHandle(string assetSubPath)
    //    {
    //        this.assetSubPath = assetSubPath;
    //    }

    //    public TValue Object
    //    {
    //        get
    //        {
    //            if (obj == null)
    //            {
    //                obj = assetSubPath.Load<TValue>();
    //            }
    //            return obj;
    //        }
    //    }
    //    private TValue obj;
    //    public bool HasObject { get { return } }
    //}

    //public static class ReadHandleExtensions
    //{
    //    public static IReadHandle<TValue> Handle<TValue>(this string assetSubPath)
    //    {
    //        return new AssetReadHandle<TValue>(assetSubPath);
    //    }
    //}
}
