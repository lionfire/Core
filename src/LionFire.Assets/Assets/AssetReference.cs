using LionFire.Referencing;

namespace LionFire.Persistence.Assets
{
    public class AssetReference<T> : ReferenceBase
        where T : class
    {
        #region AssetSubPath

        public string AssetSubPath
        {
            get { return assetSubPath; }
            set
            {
                if (assetSubPath == value) return;
                if (assetSubPath != default(string)) throw new AlreadySetException();
                assetSubPath = value;
            }
        }
        private string assetSubPath;

        #endregion
        
        public AssetReference() { }
        public AssetReference(string assetSubPath) {
            AssetSubPath = assetSubPath;
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
