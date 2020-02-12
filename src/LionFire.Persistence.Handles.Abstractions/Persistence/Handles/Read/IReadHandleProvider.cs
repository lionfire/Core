#nullable enable
using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    // TODO
    public interface IReadHandleCreator
    {
        IReadHandle<T> CreateReadHandle<T>(IReference reference);
    }
    public interface IReadHandleCreator<TReference>
        where TReference : IReference
    {
        IReadHandle<T> CreateReadHandle<T>(TReference reference);
    }

    public interface IReadHandleProvider
    {
        /// <summary>
        /// Get a read handle.  It may be a shared handle.  To ensure a new handle, use IReadHandleCreator.CreateReadHandle instead.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <returns></returns>
        /// <remarks>
        /// Example implementation:
        ///   IReadHandle<T> IReadHandleProvider.GetReadHandle<T>(IReference reference) => (reference is FileReference fileReference) ? GetReadHandle<T>(fileReference) : null;
        /// </remarks>
        IReadHandle<T>? GetReadHandle<T>(IReference reference);
    }

    //[AutoRegister]
    public interface IReadHandleProvider<TReference> : IReadHandleProvider
        where TReference : IReference
    {
        IReadHandle<T> GetReadHandle<T>(TReference reference);
    }

}
