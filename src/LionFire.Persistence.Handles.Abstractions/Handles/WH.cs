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

    //    public void DiscardObject() => throw new NotImplementedException();
    //    public Task<bool> TryResolveObject() => throw new NotImplementedException();
    //}


    public interface WH<in T> : IWriteWrapper<T>, IWrapper
    {
    }

}
