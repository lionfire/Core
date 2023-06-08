using LionFire.Persistence.Handles;
using LionFire.Data.Async.Gets;
using LionFire.Structures;

namespace LionFire.Persistence
{
    public interface IContravariantWriteHandleBase<in T> : IHandleBase, IDefaultableWriteWrapper<T>, ISets, IDiscardableValue
    {
    }

}
