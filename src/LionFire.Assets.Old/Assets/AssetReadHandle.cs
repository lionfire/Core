using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Assets;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Persistence;
using LionFire.Data.Async.Gets;
using MorseCode.ITask;

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
    public class AssetReadHandle<T> : ReadHandle<IReference, T>
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

        protected override ITask<IGetResult<T>> ResolveImpl() => throw new NotImplementedException();

        //protected override ITask<IGetResult<T>> ResolveImpl()
        //{

        //    throw new NotImplementedException("TODO: Convert Assets to an OBus");
        //    //// TODO: Use async/await here once IAssetProvider supports it
        //    //var ap = Dependencies.GetServiceOrSingleton<IAssetProvider>(createIfMissing: true);
        //    //var result = await ap.Load<T>(this.Key);
        //    //if (result != null)
        //    //{
        //    //    this.Object = result;
        //    //    return Task.FromResult(true);
        //    //}
        //    //else
        //    //{
        //    //    return Task.FromResult(false);
        //    //}
        //}
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
