using LionFire.Events;
using LionFire.Persistence.Handles;
using LionFire.Data.Async.Gets;
using LionFire.Structures;
using LionFire.Data.Async.Sets;
using LionFire.Data;

namespace LionFire.Persistence;

/// <summary>
/// IWriteHandleEx
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IWriteHandle<T> : IWriteHandleBase<T>, IWriteHandle, ISetter<T>, IStagesSet<T>
{
}

/// <summary>
/// Limited interface for when generic interface type is not available
/// </summary>
public interface IWriteHandle : ISetter, IDeletable, IDiscardableValue, IHandleBase
{

}
