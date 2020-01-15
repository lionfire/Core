using LionFire.Events;
using LionFire.Persistence.Handles;
using LionFire.Resolves;
using LionFire.Structures;

namespace LionFire.Persistence
{
    /// <summary>
    /// IWriteHandleEx
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWriteHandle<T> : IWriteHandleBase<T>, IWriteHandle, IPuts<T>
    {
    }

    /// <summary>
    /// Limited interface for when generic interface type is not available
    /// </summary>
    public interface IWriteHandle : IPuts, IDeletable, IDiscardableValue, IHandleBase
    {

    }
}
