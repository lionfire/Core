using LionFire.Referencing;
using LionFire.Structures;
using System;
using System.Threading.Tasks;

namespace LionFire
{

    ///// <summary>
    ///// ReadHandle -- Lazily (or manually) loads an Object 
    ///// </summary>
    ///// <typeparam name="TValue"></typeparam>
    //[Obsolete("Use RH or IReadHandleEx")]
    //public interface IReadHandle<out TValue> : IReadWrapper<TValue>, IWrapper, IResolvableHandle
    //{
    //    event Action<IReadHandle<TValue>, TValue /*oldValue*/ , TValue /*newValue*/> ObjectChanged;
    //}

    /// <summary>
    /// Initialize: returns true if Object loaded successfully, false if null object loaded, exception on failure or object not found
    /// </summary>


////#if UNITY
//   // REVIEW: Does covariance still crash Unity?

//        //#if !AOT
//        /// <summary>
//        /// Covariant version of IHandle&lt;TValue&gt;
//        /// </summary>
//        /// <typeparam name="TValue"></typeparam>
//        public interface IReadHandle<
//#if !UNITY
//            out // Crashes unity???
//#endif
//    TValue> :
//            //IReadHandleEx<TValue>,
//            IReadHandle
//            where TValue : class
//    {
//        new TValue Object { get; }
//    }
//#endif
    ////#endif

}
