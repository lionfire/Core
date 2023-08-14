using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace LionFire.Data.Async.Gets;

public abstract class GetterRxO<TValue>
    : ReactiveObject
    , IGetterRxO<TValue>
{
    #region Parameters

    #region (static)

    public static GetterOptions DefaultOptions { get; set; } = new();

    #endregion

    public GetterOptions GetOptions { get; set; }
    //GetterOptions IHasNonNull<GetterOptions>.Object => throw new NotImplementedException();
    //GetterOptions IHasNonNullSettable<GetterOptions>.Object { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public virtual IEqualityComparer<TValue> EqualityComparer => GetterOptions<TValue>.DefaultEqualityComparer;

    #endregion

    #region Lifecycle

    protected GetterRxO() : this(DefaultOptions) { }

    protected GetterRxO(GetterOptions? options)
    {
        GetOptions = options ?? DefaultOptions;

        // TODO REVIEW - ReactiveUI docs say for critical code, instead of ObservableAsPropertyHelper use [Reactive] and BindTo, since this doesn't get set until change
        hasValue = ((IObservableGetOperations<TValue>)this).GetResults.Select(r => r.HasValue).ToProperty(this, x => x.HasValue);
        readCacheValue2 = ((IObservableGetOperations<TValue?>)this).GetResults.Select(r => r.Value).ToProperty(this, x => x.ReadCacheValue2);
    }

    #endregion

    #region Value

    public TValue? Value
    {
        [Blocking(Alternative = nameof(GetIfNeeded))]
        get
        {
            var result = ReadCacheValue;
            if (result == null)
            {
                result = GetIfNeeded().Result.Value; // BLOCKING
                ReadCacheValue = result;
            }
            return result;
        }
    }


    public bool HasValue => hasValue.Value;
    private readonly ObservableAsPropertyHelper<bool> hasValue;

    #region ReadCacheValue

    [Reactive]
    public TValue? ReadCacheValue { get; set; }
    public TValue ReadCacheValue2 => readCacheValue2.Value;
    private readonly ObservableAsPropertyHelper<TValue?> readCacheValue2;

    #endregion

    public IGetResult<TValue> QueryValue() => new GetResult<TValue>(ReadCacheValue, HasValue);

    public void Discard() => DiscardValue();
    public void DiscardValue()
    {
        ReadCacheValue = default;
        //HasValue = false;
        getOperations.OnNext(Task.FromResult<IGetResult<TValue>>(GetResult<TValue>.Discarded).AsITask());
    }

    #endregion

    #region Get

    protected abstract ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default);

    public bool IsGetting => getState != null && getState.AsTask().IsCompleted == false;

    //public ITask<IGetResult<TValue>>? GetState => getState;
    private ITask<IGetResult<TValue>>? getState => getOperations.Value;

    public IObservable<ITask<IGetResult<TValue>>> GetOperations => getOperations;
    BehaviorSubject<ITask<IGetResult<TValue>>> getOperations = new(Task.FromResult((IGetResult<TValue>)NoopGetResult<TValue>.Instantiated).AsITask());

    SemaphoreSlim getSemaphore = new(1, 1);
    public virtual async ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        // This semaphore is to ensure only one operation active at a time (otherwise, see: https://stackoverflow.com/questions/24049931/making-an-iobservablet-that-uses-async-await-return-completed-tasks-in-origina)

        ITask<IGetResult<TValue>>? task = ReturnResultInProgress();
        if (task != null) return await task.ConfigureAwait(false); // Already in progress

        try
        {
            await getSemaphore.WaitAsync(cancellationToken);

            task = ReturnResultInProgress();
            if (task != null) return await task.ConfigureAwait(false); // Already in progress
            else
            {
                var iTask = GetImpl(cancellationToken);
                getOperations.OnNext(iTask);
                task = iTask;
            }
        }
        finally
        {
            getSemaphore.Release();
        }

        var result = await task.ConfigureAwait(false);

        // ENH TODO: based on options, don't accept failed results, retry, or never forget a successful result, etc.
        ReadCacheValue = result.IsSuccess == true ? result.Value : default;
        //HasValue = result.IsSuccess == true;
        return result;

        #region (local)

        ITask<IGetResult<TValue>>? ReturnResultInProgress()
        {
            var getTask = getState;
            if (getTask != null && !getTask.AsTask().IsCompleted) { return getTask; }
            return null;
        }

        #endregion
    }

    public async ITask<IGetResult<TValue>> GetIfNeeded()
    {
        if (HasValue) { return QueryValue(); }
        var result = await Get().ConfigureAwait(false);
        return new GetResult<TValue>(result.Value, result.IsSuccess == true);
    }

    #endregion
}

