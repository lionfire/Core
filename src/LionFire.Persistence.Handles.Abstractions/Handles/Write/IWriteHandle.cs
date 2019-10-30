using LionFire.Events;
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

    public interface IWriteHandle<in T> : IWriteWrapper<T>, IWrapper, IDiscardableValue
    {
    }

    public interface IWriteHandleEx<in T> : IWriteHandle<T>
    {
    }

    public interface INotifyingWriteHandle<T> : IWriteHandleEx<T>, INotifyPersistence, INotifyChanged<T>
    {
    }

    public interface INotifyingWriteHandleEx<out T> : INotifyingWriteHandle<T>, INotifyValueContentsChanged<T>
    {
    }

    
}
