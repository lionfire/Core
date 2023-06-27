using LionFire.Data.Async.Reactive;
using LionFire.Results;
using System.ComponentModel;
using System.Reactive.Subjects;

namespace LionFire.Data.Async;

public abstract class AsyncValue<TValue>
    : AsyncGets<TValue>
    , IAsyncValueRx<TValue>
{    
    #region Options

    #region (static)

    public new static AsyncValueOptions DefaultOptions { get; set; } = new();

    #endregion

    public new AsyncValueOptions Options
    {
        get => (AsyncValueOptions)base.Options;
        set => base.Options = value;
    }
    AsyncValueOptions IHasNonNullSettable<AsyncValueOptions>.Object { get => Options; set => Options = value; }

    AsyncValueOptions IHasNonNull<AsyncValueOptions>.Object => Options;

    #endregion

    #region Lifecycle

    public AsyncValue() : base(DefaultOptions)
    {
        asyncSets = new();        
    }

    public AsyncValue(AsyncValueOptions options) : base(options)
    {
        asyncSets = new(options);

        this.ObservableForProperty(t => t.Value)
            .Subscribe(t =>
            {
                if (HasStagedValue)
                {
                    ValueChangedWhileValueStaged = true;
                }
            });
    }

    #endregion

    #region Get+Set 

    [Reactive]
    public bool ValueChangedWhileValueStaged { get; set; }

    #endregion

    #region Get (override)

    public override ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        var setState = SetState;
        if (IsSetStateSetting(setState) && Options.OptimisticGetWhileSetting)
        {
            // return Optimistically
            return Task.FromResult<IGetResult<TValue>>(new OptimisticGetResult<TValue>(setState.SettingToValue)).AsITask();
        }
        return base.Get(cancellationToken);
    }

    #endregion

    AsyncSets<TValue> asyncSets;
    #region Set

    #region State

    [Reactive]
    public TValue? StagedValue { get; set; }

    [Reactive]
    public bool HasStagedValue { get; set; }

    public void DiscardStagedValue()
    {
        using var _ = DelayChangeNotifications();
        HasStagedValue = false;
        StagedValue = default;
        ValueChangedWhileValueStaged = false;
    }

    #endregion

    #region Status

    #endregion

    #region Events

    [Browsable(false)]
    public IObservable<ITask<ITransferResult>> Sets => sets ??= new();
    private Subject<ITask<ITransferResult>>? sets;

    #endregion

    #region Methods

    public abstract Task<ITransferResult> SetImpl(CancellationToken cancellationToken = default);
    
    public async Task<ITransferResult> Set(CancellationToken cancellationToken = default)
    {
        var result = await SetImpl(StagedValue, cancellationToken);
        if(result.IsSuccess())
        {
            HasStagedValue = false;
        }
    }

    public Task<ITransferResult> Set(TValue? value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    #endregion

    #endregion

}
