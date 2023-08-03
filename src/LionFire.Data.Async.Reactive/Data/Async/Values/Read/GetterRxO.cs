
using System.Reactive.Subjects;
using System.Threading;
using System.Xml.Linq;

namespace LionFire.Data.Async;

public abstract class GetterRxO<TValue>
    : ReactiveObject
    , IGetterRxO<TValue>
{
    #region Parameters

    #region (static)

    public static GetterOptions DefaultOptions { get; set; } = new();

    #endregion

    public GetterOptions GetOptions { get; set; }

    GetterOptions IHasNonNullSettable<GetterOptions>.Object { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public virtual IEqualityComparer<TValue> EqualityComparer => GetterOptions<TValue>.DefaultEqualityComparer;

    #endregion

    #region Lifecycle

    protected GetterRxO()
    {
        GetOptions = DefaultOptions;
    }

    protected GetterRxO(GetterOptions? options)
    {
        GetOptions = options ?? DefaultOptions;
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

    [Reactive]
    public TValue? ReadCacheValue { get; set; }

    //public TValue? ReadCacheValue
    //{
    //    [Blocking(Alternative = nameof(GetIfNeeded))]
    //    get
    //    {
    //        var result = readCacheValue;
    //        if (result == null)
    //        {
    //            result = GetIfNeeded().Result.Value; // BLOCKING
    //            ReadCacheValue = result;
    //        }
    //        return result;
    //    }
    //    protected set => this.RaiseAndSetIfChanged(ref readCacheValue, value);
    //}
    //private TValue? readCacheValue;

    //public string ReadCacheValue => readCacheValue.Value;
    //readonly ObservableAsPropertyHelper<TValue?> readCacheValue;

    [Reactive]
    public bool HasValue { get; private set; }
    public IGetResult<TValue> QueryValue() => new GetResult<TValue>(HasValue, ReadCacheValue);

    public void Discard() => DiscardValue();
    public void DiscardValue()
    {
        ReadCacheValue = default;
        HasValue = false;
    }

    #endregion

    #region Get

    protected abstract ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default);

    public bool IsGetting => getState != null && getState.AsTask().IsCompleted == false;
    public ITask<IGetResult<TValue>>? GetState => getState;

    GetterOptions IHasNonNull<GetterOptions>.Object => throw new NotImplementedException();

    public IObservable<ITask<IGetResult<TValue>>> GetOperations => getOperations;
    BehaviorSubject<ITask<IGetResult<TValue>>> getOperations = new(Task.FromResult(NoopGetResult<TValue>.Instantiated));
    public IObservable<IGetResult<TValue>> GetResults => getResults;
    Subject<IGetResult<TValue>> getResults = new();

    private ITask<IGetResult<TValue>>? getState;

    public virtual async ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        // TODO: Triage logic from AsyncGets_NoBase.Get

        var getTask = GetState;
        if (getTask != null && getState.AsTask().IsCompleted) { return await getTask.ConfigureAwait(false); }

        var task = GetImpl();
        getOperations.OnNext(task);

        var result = await task.ConfigureAwait(false);
        getResults.OnNext(result);

        ReadCacheValue = result.IsSuccess == true ? result.Value : default;
        HasValue = result.IsSuccess == true;

        return result;
    }

    public async ITask<IGetResult<TValue>> GetIfNeeded()
    {
        if (HasValue) { return QueryValue(); }
        var result = await Get().ConfigureAwait(false);
        return new GetResult<TValue>(result.IsSuccess == true, result.Value);
    }

    #endregion
}

