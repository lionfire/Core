namespace LionFire.Persistence
{
    using LionFire.Persistence.Handles;
    using LionFire.Resolves;
    using LionFire.Structures;

    /// <summary>
    /// IReadWriteHandle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadWriteHandleBase<T> : IHandleBase, IReadHandleBase<T>, IWrapper<T>, IPuts, IDeletable, IWriteHandleBase<T> { }

    /// <summary>
    /// Limited interface for when generic interface type is not available
    /// </summary>
    public interface IReadWriteHandle : IReadHandle, IWriteHandle { }

    /// <summary>
    /// IReadWriteHandleEx
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadWriteHandle<T> : IReadHandle<T>, IReadWriteHandleBase<T>, IReadWriteHandle { }

    // REVIEW: Notifying interfaces

    public interface INotifyingReadWriteHandle<T> : IReadWriteHandleBase<T>, INotifyPersists<T>
        //, INotifyChanged<T>
        //where T : class
    {
    }

    public interface INotifyingReadWriteHandleEx<T> : IReadHandle<T>, INotifyingReadWriteHandle<T>
        //where T : class
    {
    }
}
