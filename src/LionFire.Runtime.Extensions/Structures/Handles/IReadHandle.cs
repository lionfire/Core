using LionFire.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire
{
    
    //public interface IReadHandle : IInitializable
    //{
    //    object Object { get; }
    //}

    /// <summary>
    /// Initialize: returns true if Object loaded successfully, false if null object loaded, exception on failure or object not found
    /// </summary>
    public interface IReadHandle<out T> 
    //: IReadHandle
    {
         T Object { get; }

        bool HasObject { get; }
    }

    public static class IReadHandleExtensions
    {
        public static async Task<bool> TryLoad(this IReadHandle<object> rh)
        {
            return await Task.Factory.StartNew(() =>
            {
                var _ = rh.Object;
                return true;
            }).ConfigureAwait(false);
        }
        public static async Task<bool> TryLoadNonNull(this IReadHandle<object> rh)
        {
            return await Task.Factory.StartNew(() =>
            {
                var _ = rh.Object;
                return _ != null;
            }).ConfigureAwait(false);
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