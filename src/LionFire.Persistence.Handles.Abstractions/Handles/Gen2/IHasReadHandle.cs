// TODO: Move to LionFire namespace?  

namespace LionFire.Persistence.Handles
{
#if !AOT
    public interface IHasReadHandle<out T> 
        //: IHasReadHandle - needed?
        where T : class
    {
        //new
            RH<T> ReadHandle { get; }
    }
#endif

    // OLD - Unity version
//#if !AOT
//    public interface IHasReadHandle<
//#if !UNITY
//out
//#endif
// TValue> : IHasReadHandle
//        where TValue : class
//    {
//        new IReadHandle<TValue> ReadHandle { get; }
//    }
//#endif

}
