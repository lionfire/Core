namespace LionFire.Persistence
{
    using LionFire.Persistence.Handles;
    using LionFire.Referencing;
    using LionFire.Resolves;
    using LionFire.Structures;
    using System;

    /// <summary>
    /// IReadWriteHandle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadWriteHandleBase<T> : IHandleBase, IReadHandleBase<T>, IWrapper<T>, ISets, IDeletable, IWriteHandleBase<T> { }

    /// <summary>
    /// Limited interface for when generic interface type is not available
    /// </summary>
    public interface IReadWriteHandle : IReadHandle, IWriteHandle { }

    /// <summary>
    /// IReadWriteHandleEx
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadWriteHandle<T> : IReadHandle<T>, IWriteHandle<T>, IReadWriteHandleBase<T>, IReadWriteHandle, IWritableLazilyResolves<T> , IReferencableAsValueType<T>
    { }

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

    public static class IReadWriteHandleAutoSaveExtensions
    {
        public static IReadWriteHandle<T> AutoSave<T>(this IReadWriteHandle<T> handle, bool? autoSave = true)
        {
            throw new NotImplementedException();
        }
    }
}
