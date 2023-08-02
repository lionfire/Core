using LionFire.Data.Sets;
using Microsoft.Extensions.Options;
using System.Reactive.Subjects;

namespace LionFire.Data;

public abstract class Sets<TValue>
    : ReactiveObject
    , ISetsRx<TValue>
{
    #region Parameters

    #region (static)

    public static AsyncSetOptions DefaultOptions => AsyncSetOptions<TValue>.Default;

    #endregion

    public AsyncSetOptions Options { get; }
    //AsyncValueOptions IHasNonNull<AsyncValueOptions>.Object => Options;

    //AsyncGetOptions IHasNonNullSettable<AsyncSetOptions>.Object { get => Options; set => Options = value; }

    public virtual IEqualityComparer<TValue> EqualityComparer => EqualityComparerOptions<TValue>.Default;

    #endregion

    #region Lifecycle

    public Sets() : this(null) { }

    public Sets(AsyncSetOptions? options)
    {
        Options = options ?? AsyncSetOptions<TValue>.Default;
    }

    #endregion

    #region State

    public TValue? StagedValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool HasStagedValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    #endregion

    #region Events

    public IObservable<ISetOperation<TValue>> SetOperations => sets;
    private BehaviorSubject<ISetOperation<TValue>> sets = new BehaviorSubject<ISetOperation<TValue>>(new SetOperation<TValue>(default, Task.FromResult(TransferResult.Initialized).AsITask()));

    #endregion

    #region Methods

    public abstract Task<ITransferResult> Set(TValue? value, CancellationToken cancellationToken = default);

    public void DiscardStagedValue()
    {
        HasStagedValue = false;
        StagedValue = default;
    }

    public Task<ITransferResult> Set(CancellationToken cancellationToken = default)
    {
        var value = StagedValue;

        throw new NotImplementedException();
    }

    #endregion

}
