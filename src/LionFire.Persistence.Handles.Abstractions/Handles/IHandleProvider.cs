using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public interface IHandleProvider
    {
        H<T> GetHandle<T>(IReference reference, T handleObject = default);
    }

    //[AutoRegister]
    public interface IHandleProvider<TReference>
        where TReference : IReference
    {
        H<T> GetHandle<T>(TReference reference, T handleObject = default);
    }

    
    //public static class IHandleProviderExtensions
    //{
    //    // Do these casts work, or
    //    //H GetHandle(IReference reference, T handleObject = default(T)); // Needed? and R version?
    //    public static H GetHandle(this IHandleProvider handleProvider, IReference reference/*, object handleObject = null*/) => (H)handleProvider.GetHandle<object>(reference/*, handleObject*/);
    //    public static RH GetReadHandle(this IHandleProvider handleProvider, IReference reference) => (RH)handleProvider.GetReadHandle<object>(reference);
    //}

}
