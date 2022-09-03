#nullable enable
using LionFire.UI;
//using Microsoft.Extensions.Options;
//using static LionFire.Reflection.GetMethodEx;

namespace LionFire.UI;

public abstract class AsyncViewModel<T> : IDependsOn<T>, IReadWrapper<T>
    where T : class
{

    public AsyncPropertyOptions? Options { get; set; }

    public T Model { get => model; set { if (model != default) { throw new AlreadySetException(); } model = value; } }
    private T model;
    T? IReadWrapper<T>.Value => Model;

    public T Dependency { set => Model = value; }


    #region Construction

    public AsyncViewModel() { }
    public AsyncViewModel(T model)
    {
        Model = model;
    }

    #endregion
}

//public class ConsistentAsyncProperty<TProperty>
//{
//    public TProperty LastValueFromSource { get; private set; }
//}
