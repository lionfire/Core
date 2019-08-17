using LionFire.Referencing;
using LionFire.Structures;
using System;
using System.Threading.Tasks;

namespace LionFire
{

    /// <summary>
    /// ReadHandle -- Lazily (or manually) loads an Object 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadHandle<out T> : IReadWrapper<T>, IResolvableHandle
    {
        event Action<IReadHandle<T>, T /*oldValue*/ , T /*newValue*/> ObjectChanged;
    }

    /// <summary>
    /// Initialize: returns true if Object loaded successfully, false if null object loaded, exception on failure or object not found
    /// </summary>


////#if UNITY
//   // REVIEW: Does covariance still crash Unity?

//        //#if !AOT
//        /// <summary>
//        /// Covariant version of IHandle&lt;T&gt;
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        public interface IReadHandle<
//#if !UNITY
//            out // Crashes unity???
//#endif
//    T> :
//            //IReadHandleEx<T>,
//            IReadHandle
//            where T : class
//    {
//        new T Object { get; }
//    }
//#endif
    ////#endif

}
