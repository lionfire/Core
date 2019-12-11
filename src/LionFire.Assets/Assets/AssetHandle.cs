using System;
using System.Threading.Tasks;
using LionFire.Assets;
using LionFire.Dependencies;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Implementation;
using LionFire.Referencing;

namespace LionFire.Persistence.Assets
{
    // TODO: Convert to HandlePersister approach

    public class AssetHandle<T> : AssetReadHandle<T>
        //, IHandleImpl<T> TODO
        where T : class
    {
        #region Construction

        public AssetHandle() { }
        public AssetHandle(string subPath) : base(subPath) { }

        public AssetHandle(IReference reference, T handleObject = null) : base(reference, handleObject)
        {
        }

        #endregion

        public void MarkDeleted() => throw new System.NotImplementedException();
        public Task<bool?> Delete() => throw new System.NotImplementedException();

        public Task Commit()
        {
            throw new NotImplementedException();
#if TODO
            var ap = DependencyLocator.TryGet<IAssetProvider>(tryCreateIfMissing: true);
            ap.Save(this.Key, this.Value);
            return Task.CompletedTask;
#endif
            }

#if TODO
        public void SetObject(T obj) { base.Value = obj; }
        Task<bool> IDeletable.Delete() => throw new System.NotImplementedException();
        Task<IPersistenceResult> ICommitableImpl.Commit() => throw new System.NotImplementedException();
        Task<IPersistenceResult> IDeletableImpl.Delete() => throw new System.NotImplementedException();
#endif
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
