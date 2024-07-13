using LionFire.Data.Async.Sets;
using LionFire.IO;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace LionFire.Data.Async.Gets;

public abstract class GetterRxO<TValue>
    : ReactiveObject
    , IGetterRxO<TValue>
    , IValueState<TValue>
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
        readCacheValue2 = ((IObservableGetOperations<TValue?>)this).GetResults.Select(r =>
        {
            Debug.WriteLine(r.ToDebugString());
            return r.Value;
        }).ToProperty(this, x => x.ReadCacheValue2);

        this.ObservableForProperty(x => x.ReadCacheValue)
            .Subscribe(x => this.RaisePropertyChanged(nameof(Value)));

#if DEBUG
        this.PropertyChanged += OnPropertyChanged_Debug;
#endif
    }

#if DEBUG
    private void OnPropertyChanged_Debug(object sender, PropertyChangedEventArgs e)
    {
        foreach (var pi in sender.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).Where(pi => pi.Name == e.PropertyName))
        {
            try
            {
                var val = pi.GetValue(sender)?.ToString() ?? "(null)";
                Debug.WriteLine($"GetterRxO<{typeof(TValue).Name}>.PropertyChanged: {e.PropertyName} = {val}");
            }
            catch { }


        }

    }
#endif

    #endregion

    #region Value

    public TValue? Value
    {
        [Blocking(Alternative = nameof(GetIfNeeded))]
        get
        {
            if (!HasValue && GetOptions.BlockToGet)
            {
                return GetIfNeeded().Result.Value; // BLOCKING
            }
            return ReadCacheValue;
        }
    }

    #region IValueState

    TValue? IValueState<TValue>.Value { get => Value; set => throw new NotSupportedException(); }

    [Reactive]
    public ValueStateFlags StateFlags { get; protected set; }

    public virtual IODirection IODirection => IODirection.Read;

    #endregion

    public bool HasValue => hasValue.Value;
    private readonly ObservableAsPropertyHelper<bool> hasValue;

    #region ReadCacheValue

    [Reactive]
    public TValue? ReadCacheValue { get; set; }
    public TValue? ReadCacheValue2 => readCacheValue2.Value;
    private readonly ObservableAsPropertyHelper<TValue?> readCacheValue2;

    #endregion

    public IGetResult<TValue> QueryGetResult() => GetResult<TValue>.QueryValue(ReadCacheValue, HasValue);

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

    protected BehaviorSubject<ITask<IGetResult<TValue>>> getOperations = new(Task.FromResult((IGetResult<TValue>)NoopGetResult<TValue>.Instantiated).AsITask());

    SemaphoreSlim getSemaphore = new(1, 1);

    protected virtual void OnGetResult(IGetResult<TValue> result)
    {
        Debug.WriteLine($"GetterRxO.ReadCacheValue = {ReadCacheValue2} {HasValue} (was {ReadCacheValue})");
        ReadCacheValue = ReadCacheValue2;
    }

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
                iTask = iTask.AsTask().ContinueWith(x =>
                {
                    OnGetResult(x.Result);
                    return x.Result;
                }).AsITask();
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
        if (HasValue) { return QueryGetResult(); }
        var result = await Get().ConfigureAwait(false);
        return result;
        //return new GetResult<TValue>(result.Value, result.IsSuccess == true) { Flags = result.Flags };
    }

    #endregion


    #region Non-existant Write support

    void IWriteStagesSet.DiscardStagedValue() => throw new NotSupportedException();

    Task<ISetResult> ISetter.Set(CancellationToken cancellationToken) => throw new NotSupportedException();

    #endregion


    // Exposed only on derived classes that are read-write IAwareOfSets
    public virtual void OnSet(ISetResult<TValue> setResult) 
    {
        // REVIEW: are there any race condition issues with Gets in progress?  Perhaps derived event notification ordering
        getOperations.OnNext(Task.FromResult<IGetResult<TValue>>(GetResult<TValue>.FromSet(setResult)).AsITask());
    }
}

