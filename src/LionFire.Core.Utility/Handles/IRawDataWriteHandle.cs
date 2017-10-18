namespace LionFire
{
    public interface IRawDataWriteHandle
    {
        object RawData { set; }
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
