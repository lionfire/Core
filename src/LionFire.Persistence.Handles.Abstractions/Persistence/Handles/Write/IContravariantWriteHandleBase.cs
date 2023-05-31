using LionFire.Persistence.Handles;
using LionFire.Resolves;
using LionFire.Structures;

namespace LionFire.Persistence
{
    public interface IContravariantWriteHandleBase<in T> : IHandleBase, IDefaultableWriteWrapper<T>, ISets, IDiscardableValue
    {
    }

}
