#nullable enable

namespace LionFire.Mvvm;

public abstract class AsyncVM<T> : ReactiveObject, IDependsOn<T>, IViewModel<T>
    where T : class
{

    public AsyncPropertyOptions? Options { get; set; }

    public T Model { get => model; set { if (model != default) { throw new AlreadySetException(); } model = value; } }
    private T model;

    #region Explicit interfaces

    T? IReadWrapper<T>.Value => Model;
    T IDependsOn<T>.Dependency { set => Model = value; }

    #endregion


    #region Construction

    //public AsyncVM() { }
    public AsyncVM(T model)
    {
        Model = model;
    }

    #endregion
}

