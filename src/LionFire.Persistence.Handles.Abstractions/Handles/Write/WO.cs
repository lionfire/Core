using LionFire.Persistence.Handles;
using LionFire.Resolves;
using LionFire.Structures;

namespace LionFire.Persistence
{
    /// <summary>
    /// IWriteHandle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface WO<T> : IHandleBase, IWrapper<T>, IPuts, IWriteHandle<T>
    {

    }

    /// <summary>
    /// IWriteHandleEx
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface WOx<T> : WO<T>, IDeletable
    {
    }
}
