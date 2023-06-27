using LionFire.Data.Async.Reactive;
using LionFire.Results;
using MorseCode.ITask;
using System.Reactive.Subjects;

namespace LionFire.Data.Async;

public abstract class AsyncValue<TKey, TValue>
    : AsyncGets<TKey, TValue>
    , IAsyncValueRx<TValue>
{
    #region Options

    #region (static)

    public static new AsyncValueOptions DefaultOptions => AsyncValueOptions<TKey, TValue>.Default;

    #endregion

    public new AsyncValueOptions Options
    {
        get => (AsyncValueOptions)base.GetOptions;
        set => base.GetOptions = value;
    }
    AsyncValueOptions IHasNonNullSettable<AsyncValueOptions>.Object { get => Options; set => Options = value; }

    AsyncValueOptions IHasNonNull<AsyncValueOptions>.Object => Options;

    #endregion

    #region Lifecycle

    public AsyncValue(TKey key, AsyncGetOptions? options = null) : base(key, options ?? DefaultOptions)
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

    public IObservable<ITask<ITransferResult?>> Sets => sets;
    private BehaviorSubject<ITask<ITransferResult?>> sets = new (Task.FromResult<ITransferResult?>(null).AsITask());

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
