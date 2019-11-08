using LionFire.Persistence.Handles;
using LionFire.Resolves;
using LionFire.Structures;

namespace LionFire.Persistence
{
    //public class ConcreteHandle : H<object>
    //{
    //    public object Object { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    //    public bool IsResolved => throw new NotImplementedException();

    //    public bool HasObject => throw new NotImplementedException();

    //    object IReadWrapper<object>.Object => throw new NotImplementedException();

    //    object IWriteWrapper<object>.Object { set => throw new NotImplementedException(); }

    //    public event Action<bool> IsResolvedChanged;

    //    public void DiscardValue() => throw new NotImplementedException();
    //    public Task<bool> TryResolveObject() => throw new NotImplementedException();
    //}

    //public interface IWriteHandle<in T> : IWriteWrapper<T>, IWrapper, IDiscardableValue

    //{
    //}

    //public interface IWriteHandleEx<in T> : IWriteHandle<T>
    //{
    //}

    /// <summary>
    /// IWriteHandle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWriteHandleBase<T> : IContravariantWriteHandleBase<T>, IWriteWrapper<T>, IWrapper
    {
    }

    public interface IContravariantWriteHandleBase<in T> : IHandleBase, IWriteWrapper<T>, IPuts, IDiscardableValue
    {
    }

}
