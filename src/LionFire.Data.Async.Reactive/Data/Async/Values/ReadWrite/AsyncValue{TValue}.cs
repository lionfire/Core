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
    }

    public AsyncValue(AsyncValueOptions options) : base(options)
    {
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

    #region Events

    [Browsable(false)]
    public IObservable<ITask<ITransferResult>> Sets => sets ??= new();
    private Subject<ITask<ITransferResult>>? sets;

    #endregion

    #region Methods

    public Task<ITransferResult> Set(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
        //try
        //{

        //}
        //catch ()
        //{

        //}
    }

    public Task<ITransferResult> Set(TValue? value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    #endregion

    #endregion

}
