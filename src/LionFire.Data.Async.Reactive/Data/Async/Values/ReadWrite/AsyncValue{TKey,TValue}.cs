using LionFire.Data.Reactive;
using LionFire.Results;
using MorseCode.ITask;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace LionFire.Data;


public abstract class AsyncValue<TKey, TValue>
    : AsyncGets<TKey, TValue>
    , IAsyncValueRx<TValue>
{
    #region Options

    #region (static)

    public static new AsyncValueOptions DefaultOptions => AsyncValueOptions<TKey, TValue>.Default;

    public static IEqualityComparer<TValue> DefaultEqualityComparer => EqualityComparerOptions<TValue>.Default;

    #endregion

    public AsyncValueOptions Options
    {
        get => (AsyncValueOptions)base.GetOptions;
        set => base.GetOptions = value;
    }
    AsyncValueOptions IHasNonNullSettable<AsyncValueOptions>.Object { get => Options; set => Options = value; }

    AsyncValueOptions IHasNonNull<AsyncValueOptions>.Object => Options;

    #endregion

    #region Lifecycle

    public AsyncValue(TKey key, AsyncValueOptions? options = null) : base(key, options ?? DefaultOptions)
    {
        Options = options ?? AsyncObjectKey?.Options.PropertyOptions ?? DefaultOptions;

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

    #region Status

    public bool IsSetting => IsSetStateSetting(SetState);
    public static bool IsSetStateSetting((TValue? SettingToValue, ITask<ITransferResult> task) setState) => setState.task != null && setState.task.AsTask().IsCompleted == false;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// locked by: setLock
    /// </remarks>
    [Reactive]
    public (TValue? SettingToValue, ITask<ITransferResult> task) SetState => sets.Value;

    #endregion

    #region Events

    public IObservable<(TValue?, ITask<ITransferResult>)> Sets => sets;
    private BehaviorSubject<(TValue?, ITask<ITransferResult>)> sets = new((default, Task.FromResult<ITransferResult>(TransferResult.Initialized).AsITask()));

    #endregion

    #region Methods

    public async Task<ITransferResult> Set(CancellationToken cancellationToken = default)
    {
        try
        {
            await Set(StagedValue, cancellationToken);
            return SuccessResult.Success;
        }
        catch (Exception ex)
        {
            return new ExceptionResult(ex);
        }
    }

    public abstract Task<ITransferResult> SetImpl(TKey key, TValue? value, CancellationToken cancellationToken = default);

    private object setLock = new();

    /// <summary>
    /// Skips set if DefaultEqualityComparer.Equals(currentSetState.Value.SettingToValue, value)
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Set(TValue? value, CancellationToken cancellationToken = default)
    {
        var target = Key;
        ArgumentNullException.ThrowIfNull(target, nameof(Key));

        (TValue? SettingToValue, ITask<ITransferResult> task)? currentSetState;
    start:
        do
        {
            currentSetState = SetState;
            if (currentSetState.task != null) { await currentSetState.Value.task!.ConfigureAwait(false); }
            if (DefaultEqualityComparer.Equals(currentSetState.Value.SettingToValue, value)) { return; }

            // ENH: Based on option: Also wait for existing get/set to complete to avoid setting to a value that will be overwritten, or to avoid setting to a value that is the same as the gotten value
        } while (currentSetState.Value.task != null);

        lock (setLock)
        {
            if (IsSetting) goto start;
            sets.OnNext((value, SetImpl(target, value, cancellationToken)));
        }
        await SetState.task.ConfigureAwait(false);
        //OnPropertyChanged(nameof(Value));

    }

    #endregion

    #endregion
}
