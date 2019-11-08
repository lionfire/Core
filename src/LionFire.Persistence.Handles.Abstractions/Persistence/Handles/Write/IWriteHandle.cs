using LionFire.Events;
using LionFire.Resolves;
using LionFire.Structures;

namespace LionFire.Persistence
{

    /// <summary>
    /// IWriteHandleEx
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWriteHandle<T> : IWriteHandleBase<T>, IWrapper, IWrapper<T>, IPersists, IDeletable, IPuts<T>
    {
    }

}
