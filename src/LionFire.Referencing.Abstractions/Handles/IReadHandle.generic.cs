using System;
using System.Threading.Tasks;

namespace LionFire
{
    /// <summary>
    /// Initialize: returns true if Object loaded successfully, false if null object loaded, exception on failure or object not found
    /// </summary>
    public interface IReadHandle<out T> : IResolvableHandle
    {
        T Object { get; }

        event Action<IReadHandle<T>, T /*oldValue*/ , T /*newValue*/> ObjectChanged;
    }

#if Experimental
    public interface IReadHandleBase
    {
    }
    public static class IReadHandleBaseExtensions
    {
        public static IReadHandle<object> ToObjectHandle(this IReadHandleBase rhb)
        {
            var rh = (IReadHandle<object>)rhb;
            return rh;
        }
    }
#endif

#if UNITY
    REVIEW: Does covariance still crash Unity?

    ////#if !AOT
    //    /// <summary>
    //    /// Covariant version of IHandle&lt;T&gt;
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    public interface IReadHandle<
    //#if !UNITY
    //        out // Crashes unity???
    //#endif
    //T> :
    //        //IReadHandleEx<T>,
    //        IReadHandle
    //        where T : class
    //    {
    //        new T Object { get; }
    //    }
    //#endif
#endif


#if UNUSED // HACK used somewhere?
    public interface IReadHandleEx<
#if !UNITY
        out // Crashes unity???
#endif
T> : IReadHandle
        where T : class
    {
        T ObjectField { get; } // REVIEW - this seems to be a hack.  eliminate it?
    }
#endif
}
