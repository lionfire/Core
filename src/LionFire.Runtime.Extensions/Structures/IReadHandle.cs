using LionFire.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire
{

    /// <summary>
    /// Initialize: returns true if Object loaded successfully, false if null object loaded, exception on failure or object not found
    /// </summary>
    public interface IReadHandle : IInitializable
    {
        object Object { get; }
        bool HasObject { get; }
    }
    public interface IReadHandle<T> : IReadHandle
    {
        new T Object { get; }
    }

    public static class IReadHandleExtensions
    {
        public static async Task<bool> TryLoad(this IReadHandle rh)
        {
            return await Task.Factory.StartNew(() =>
            {
                var _ = rh.Object;
                return true;
            });
        }
        public static async Task<bool> TryLoadNonNull(this IReadHandle rh)
        {
            return await Task.Factory.StartNew(() =>
            {
                var _ = rh.Object;
                return _ != null;
            });
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