#nullable enable

using LionFire.Resolves;
using LionFire.Results;
using MorseCode.ITask;

namespace LionFire.Mvvm.Async;

[Obsolete("Use AsyncValueRx")] // OLD
public class SettablePropertyAsync<TObject, TValue>
    : PropertyAsync<TObject, TValue>
    , IStagesSet<TValue>
    , ISets
    , IObservableSets<TValue>
{
    #region Lifecycle

    public SettablePropertyAsync(TObject target, AsyncValueOptions? options = null) : base(target, options)
    {
    }

    #endregion

    #region Get (override)

    public override Task<TValue?> Get(CancellationToken cancellationToken = default)
    {
        var setState = SetState;
        if (IsSetting && Options.OptimisticGetWhileSetting) { return Task.FromResult(setState.SettingToValue); } // return Optimistically
        return base.Get(cancellationToken);
    }

    #endregion

    #region IsSetting

    public bool IsSetting => SetState.task != null && SetState.task.IsCompleted == false;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// locked by: setLock
    /// </remarks>
    [Reactive]
    public (Task? task, TValue? SettingToValue) SetState { get; private set; }
    //{
    //    get => setState;
    //    private set
    //    {
    //        // REVIEW - is this necessary?
    //        //bool oldIsSetting = IsSetting;
    //        //setState = value;

    //        this.RaiseAndSetIfChanged(ref setState, value);

    //        //if (IsSetting != oldIsSetting)
    //        //{
    //        //    OnPropertyChanged(nameof(IsSetting));
    //        //}
    //    }
    //}
    //private (Task? task, TValue? SettingToValue) setState;

    #endregion

    #region IStagesSet, ISets

    [Reactive]
    public TValue? StagedValue { get; set; }

    public async Task<ISuccessResult> Set(CancellationToken cancellationToken = default)
    {
        try
        {
            await Set(StagedValue, cancellationToken);
            return SuccessResult.Success;
        }
        catch(Exception ex)
        {
            return new ExceptionResult(ex);
        }
    }

    #endregion

    #region Set

    private object setLock = new();

    public Func<TObject, TValue?, CancellationToken, Task> Setter { get; set; }
    public async Task Set(TValue? value, CancellationToken cancellationToken = default)
    {

        (Task? task, TValue? SettingToValue) currentSetState;
    start:
        do
        {
            currentSetState = SetState;
            if (currentSetState.task != null) { await currentSetState.task!.ConfigureAwait(false); }
            if (EqualityComparer.Equals(currentSetState.SettingToValue, value)) { return; }

            // ENH: Based on option: Also wait for existing get/set to complete to avoid setting to a value that will be overwritten, or to avoid setting to a value that is the same as the gotten value
        } while (currentSetState.task != null);

        try
        {
            lock (setLock)
            {
                if (IsSetting) goto start;
                SetState = (Setter(Target, value, cancellationToken), value);
            }
            await SetState.task.ConfigureAwait(false);
            //OnPropertyChanged(nameof(Value));
        }
        finally
        {
            SetState = (null, default);
        }
    }

    #endregion
   
}

