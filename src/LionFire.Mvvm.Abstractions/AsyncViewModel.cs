#nullable enable
//using LionFire.UI;

namespace LionFire.Mvvm;

public abstract class AsyncVM<T> : IDependsOn<T>, IViewModel<T>
    where T : class
{

    public AsyncPropertyOptions? Options { get; set; }

    public T Value { get => model; set { if (model != default) { throw new AlreadySetException(); } model = value; } }
    private T model;
    T? IReadWrapper<T>.Value => Value;

    public T Dependency { set => Value = value; }


    #region Construction

    public AsyncVM() { }
    public AsyncVM(T model)
    {
        Value = model;
    }

    #endregion
}

