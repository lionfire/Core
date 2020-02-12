#nullable enable
using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public interface IReadWriteHandleCreator
    {
        IReadWriteHandle<T> CreateReadWriteHandle<T>(IReference reference);
    }

    public interface IReadWriteHandleCreator<TReference>
        where TReference : IReference
    {
        IReadWriteHandle<T> CreateReadWriteHandle<T>(TReference reference);
    }

    public interface IReadWriteHandleProvider
    {
        IReadWriteHandle<T>? GetReadWriteHandle<T>(IReference reference);
    }

    public interface IReadWriteHandleProvider<TReference>
        where TReference : IReference
    {
        IReadWriteHandle<T> GetReadWriteHandle<T>(TReference reference);
    }
    
    //public static class IHandleProviderExtensions
    //{
    //    // Do these casts work, or
    //    //H GetHandle(IReference reference(T)); // Needed? and R version?
    //    public static H GetHandle(this IReadWriteHandleProvider handleProvider, IReference reference/*, object handleObject = null*/) => (H)handleProvider.GetHandle<object>(reference/*, handleObject*/);
    //    public static RH GetReadHandle(this IReadWriteHandleProvider handleProvider, IReference reference) => (RH)handleProvider.GetReadHandle<object>(reference);
    //}

}
