using LionFire.Data.Async.Gets;
using LionFire.Structures;

namespace LionFire.Persistence
{
    /// <summary>
    /// IWriteHandleBase
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWriteHandleBase<T> : IContravariantWriteHandleBase<T>, IDefaultableWrapper<T>
    {
    }
}
