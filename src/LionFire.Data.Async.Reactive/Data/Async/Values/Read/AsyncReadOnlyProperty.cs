
using System.Reactive.Subjects;

namespace LionFire.Data;

public abstract class AsyncReadOnlyProperty<TObject, TValue> : Gets<TObject, TValue>
{
    #region Relationships

    public TObject Target { get; }

    public IAsyncObject? TargetAsync => Target as IAsyncObject;

    #endregion

    #region Lifecycle

    public AsyncReadOnlyProperty(TObject target, AsyncGetOptions? options = null) : base(target, options)
    {
        Target = target;
        GetOptions = options ?? TargetAsync?.Options?.ValueOptions.Get ?? DefaultOptions;
    }

    #endregion

    //public Func<TObject, CancellationToken, Task<TValue>> Getter { get; set; }

    protected override ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
