using System.Threading.Tasks;
using LionFire.Assets;
using LionFire.Referencing;

namespace LionFire.Persistence.Assets
{
    public class AssetHandle<T> : AssetReadHandle<T>, IWriteHandle<T>, W<T>
        where T : class
    {
        #region Construction

        public AssetHandle() { }
        public AssetHandle(string subPath) :base(subPath) { }

        #endregion

        public void MarkDeleted() => throw new System.NotImplementedException();

        public Task Save(object persistenceContext = null)
        {
            var ap = Injection.GetService<IAssetProvider>(createIfMissing: true);
            ap.Save(this.Key, this.Object, persistenceContext);
            return Task.CompletedTask;
        }

        public void SetObject(T obj) { base.Object = obj; }
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
