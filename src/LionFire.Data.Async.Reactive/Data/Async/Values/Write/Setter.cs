using LionFire.Data.Async.Sets;
using Microsoft.Extensions.Options;
using System.Reactive.Subjects;

namespace LionFire.Data.Async.Sets;

// ENH idea: etag support, probably within the Value<TValue> class.
// ENH idea for corner case: Setter without preknowledge of existing value, but with an asosociated IGetter<ETag>.

public abstract class Setter<TValue>
    : ReactiveObject
    , ISetterRxO<TValue>
{
    #region Parameters

    #region (static)

    public static SetterOptions DefaultOptions => SetterOptions<TValue>.Default;

    #endregion


    public SetterOptions Options { get; set; }
    //SetterOptions IHasNonNull<SetterOptions>.Object => Options;
    //SetterOptions IHasNonNullSettable<SetterOptions>.Object { get => Options; set => Options = value; }

    public virtual IEqualityComparer<TValue> EqualityComparer => EqualityComparerOptions<TValue>.Default;

    #endregion

    #region Lifecycle

    public Setter() : this(null) { }

    public Setter(SetterOptions? options)
    {
        Options = options ?? SetterOptions<TValue>.Default;
    }

    #endregion

    #region State

    public TValue? StagedValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool HasStagedValue { get; set; }

    public enum SetterStateFlags
    {
        Unspecified = 0,
        HasStagedValue = 1 << 0,
        // SuccessfullySet
        // FailedToSet
        // SetInProgress
        Expired = 1 << 0,
    }

    #endregion

    #region Events

    public IObservable<ISetOperation<TValue>> SetOperations => sets;
    private BehaviorSubject<ISetOperation<TValue>> sets = new(NoopSetOperation<TValue>.Instantiated);

    public IObservable<ISetResult<TValue>> SetResults => setResults;
    private BehaviorSubject<ISetResult<TValue>> setResults = new(NoopSetOperation<TValue>.Instantiated);

    #endregion

    #region Methods

    public async ITask<ISetResult<T>> Set<T>(T? value, CancellationToken cancellationToken = default) where T : TValue
    {
        var task = SetImpl(value, cancellationToken);
        sets.OnNext(new SetOperation<TValue>(value, task));
        var result = await task.ConfigureAwait(false);
        setResults.OnNext((ISetResult<TValue>)result);
        return result;
    }

    public abstract Task<ISetResult<T>> SetImpl<T>(T? value, CancellationToken cancellationToken = default) where T : TValue;

    public void DiscardStagedValue()
    {
        HasStagedValue = false;
        StagedValue = default;
    }

    public Task<ISetResult> Set(CancellationToken cancellationToken = default)
    {
        var value = StagedValue;
        return Set(value, cancellationToken);
    }

    #endregion

}
