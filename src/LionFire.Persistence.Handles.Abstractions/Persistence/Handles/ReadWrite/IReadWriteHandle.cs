namespace LionFire.Persistence;

using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Data.Async.Gets;
using LionFire.Structures;
using System;
using LionFire.Data.Async.Sets;

/// <summary>
/// IReadWriteHandle
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IReadWriteHandleBase<T> : IHandleBase, IReadHandleBase<T>, IWrapper<T>, ISetter, IDeletable, IWriteHandleBase<T> { }

/// <summary>
/// Limited interface for when generic interface type is not available
/// </summary>
public interface IReadWriteHandle : IReadHandle, IWriteHandle { }

/// <summary>
/// IReadWriteHandleEx
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IReadWriteHandle<T> 
    : IReadHandle<T>, IWriteHandle<T>, IReadWriteHandleBase<T>, IReadWriteHandle, IReferencableAsValueType<T>
    //, IWritableLazilyGets<T> // Where did this go?
{

    ITask<IGetResult<T>> GetOrInstantiateValue();
}


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
