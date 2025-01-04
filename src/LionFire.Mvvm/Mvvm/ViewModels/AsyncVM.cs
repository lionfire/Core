
using LionFire.Data.Async;
using LionFire.Metadata;

namespace LionFire.Mvvm;

// REVIEW - isn't this a duplicate of AsyncValue?
//
// AsyncValue
//
// implements:
// - GetterRxO
// - IAsyncValue<TValue>
// - IValueState<TValue>
//
// parameters:
// - ValueOptions
public abstract class AsyncVM<T> : ReactiveObject, IDependsOn<T>, IViewModel<T>
    where T : class
{
    #region Parameters

    [Relevance(RelevanceFlags.Technical | RelevanceFlags.Internal)]
    public ValueOptions? Options { get; set; }

    #endregion

    #region Construction

    public AsyncVM(T model)
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

