
using System.Reactive.Subjects;

namespace LionFire.Data;

public abstract class AsyncReadOnlyProperty<TObject, TValue> : AsyncGets<TObject, TValue>
{
    #region Relationships

    public TObject Target { get; }

    public IAsyncObject? TargetAsync => Target as IAsyncObject;

    #endregion

    #region Lifecycle

    public AsyncReadOnlyProperty(TObject target, AsyncValueOptions? options = null)
    {
        Target = target;
        Options = options ?? TargetAsync?.Options?.PropertyOptions ?? DefaultOptions;
    }

    #endregion

    //public Func<TObject, CancellationToken, Task<TValue>> Getter { get; set; }

    protected override ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
