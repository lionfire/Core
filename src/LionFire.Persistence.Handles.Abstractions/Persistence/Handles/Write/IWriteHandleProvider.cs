#nullable enable
using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public interface IWriteHandleCreator
    {
        IWriteHandle<T> CreateWriteHandle<T>(IReference reference, T handleObject = default);
    }

    public interface IWriteHandleCreator<TReference>
        where TReference : IReference
    {
        IWriteHandle<T> CreateWriteHandle<T>(TReference reference, T handleObject = default);
    }

    public interface IWriteHandleProvider
    {
        IWriteHandle<T>? GetWriteHandle<T>(IReference reference);
    }

    public interface IWriteHandleProvider<TReference>
        where TReference : IReference
    {
        IWriteHandle<T> GetWriteHandle<T>(TReference reference);
    }
}
