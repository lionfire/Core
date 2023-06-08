using LionFire.Events;
using LionFire.Persistence.Handles;
using LionFire.Data.Async.Gets;
using LionFire.Structures;

namespace LionFire.Persistence
{
    /// <summary>
    /// IWriteHandleEx
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWriteHandle<T> : IWriteHandleBase<T>, IWriteHandle, ISets<T>
    {
    }

    /// <summary>
    /// Limited interface for when generic interface type is not available
    /// </summary>
    public interface IWriteHandle : ISets, IDeletable, IDiscardableValue, IHandleBase
    {

    }
}
