using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public interface IWriteHandleProvider
    {
        IWriteHandleBase<T> GetWriteHandle<T>(IReference reference, T handleObject = default);
    }

    public interface IWriteHandleProvider<TReference>
        where TReference : IReference
    {
        IWriteHandleBase<T> GetWriteHandle<T>(TReference reference, T handleObject = default);
    }
}
