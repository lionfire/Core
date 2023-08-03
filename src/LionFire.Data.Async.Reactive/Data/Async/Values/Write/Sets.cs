using LionFire.Data.Async.Sets;
using Microsoft.Extensions.Options;
using System.Reactive.Subjects;

namespace LionFire.Data.Async.Sets;

public abstract class Sets<TValue>
    : ReactiveObject
    , ISetterRxO<TValue>
{
    #region Parameters

    #region (static)

    public static SetterOptions DefaultOptions => SetterOptions<TValue>.Default;

    #endregion

    public SetterOptions Options { get; }
    SetterOptions IHasNonNull<SetterOptions>.Object => Options;

    //AsyncGetOptions IHasNonNullSettable<SetterOptions>.Object { get => Options; set => Options = value; }

    public virtual IEqualityComparer<TValue> EqualityComparer => EqualityComparerOptions<TValue>.Default;

    #endregion

    #region Lifecycle

    public Sets() : this(null) { }

    public Sets(SetterOptions? options)
    {
        Options = options ?? SetterOptions<TValue>.Default;
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
