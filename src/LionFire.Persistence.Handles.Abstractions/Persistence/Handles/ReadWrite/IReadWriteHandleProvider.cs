using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public interface IReadWriteHandleProvider
    {
        IReadWriteHandleBase<T> GetReadWriteHandle<T>(IReference reference);
    }

    //[AutoRegister]
    public interface IReadWriteHandleProvider<TReference>
        where TReference : IReference
    {
        IReadWriteHandleBase<T> GetReadWriteHandle<T>(TReference reference);
    }

    
    //public static class IHandleProviderExtensions
    //{
    //    // Do these casts work, or
    //    //H GetHandle(IReference reference(T)); // Needed? and R version?
    //    public static H GetHandle(this IReadWriteHandleProvider handleProvider, IReference reference/*, object handleObject = null*/) => (H)handleProvider.GetHandle<object>(reference/*, handleObject*/);
    //    public static RH GetReadHandle(this IReadWriteHandleProvider handleProvider, IReference reference) => (RH)handleProvider.GetReadHandle<object>(reference);
    //}

}
