using System.Threading.Tasks;
using LionFire.Assets;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Implementation;
using LionFire.Referencing;

namespace LionFire.Persistence.Assets
{
    public class AssetHandle<T> : AssetReadHandle<T>, IHandleImpl<T>
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
            var ap = Dependencies.GetServiceOrSingleton<IAssetProvider>(createIfMissing: true);
            ap.Save(this.Key, this.Object);
            return Task.CompletedTask;
        }

        public void SetObject(T obj) { base.Object = obj; }

        Task<bool> IDeletable.Delete() => throw new System.NotImplementedException();
        Task<IPersistenceResult> ICommitableImpl.Commit() => throw new System.NotImplementedException();
        Task<IPersistenceResult> IDeletableImpl.Delete() => throw new System.NotImplementedException();
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
