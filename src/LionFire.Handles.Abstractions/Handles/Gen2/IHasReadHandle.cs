// TODO: Move to LionFire namespace?  
namespace LionFire.Referencing // RENAME to LionFire.Handles? or LionFire.Referencing?
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
// T> : IHasReadHandle
//        where T : class
//    {
//        new IReadHandle<T> ReadHandle { get; }
//    }
//#endif

}
