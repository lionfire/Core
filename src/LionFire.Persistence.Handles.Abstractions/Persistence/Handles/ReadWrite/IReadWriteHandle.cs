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
    /// IReadWriteHandleEx
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadWriteHandle<T> : IReadHandle<T>, IReadWriteHandleBase<T>
    {
    }

    public interface INotifyingReadWriteHandle<T> : IReadWriteHandleBase<T>, INotifyPersists<T>
        //, INotifyChanged<T>
    {
    }

    public interface INotifyingReadWriteHandleEx<T> : IReadHandle<T>, INotifyingReadWriteHandle<T>
    {
    }

}
