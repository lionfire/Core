using LionFire.Metadata;

namespace LionFire.Mvvm;

/// <summary>
/// A IViewModel that requires a Value.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ViewModelRx<T> : ReactiveObject, IDependsOn<T>, IViewModel<T>
    where T : class
{

    #region Construction

    public ViewModelRx(T model)
    {
        Value = model;
    }

    #endregion

    #region State

    [Relevance(RelevanceFlags.Internal)]
    public T? Value
    {
        get => value;
        set
        {
            if (this.value != default) { throw new AlreadySetException(); }
            this.value = value;
        }
    }
    private T? value;

    #region Explicit interfaces

    //TValue? IReadWrapper<TValue>.Value => Value;
    T IDependsOn<T>.Dependency { set => Value = value; }

    #endregion

    #endregion

}

