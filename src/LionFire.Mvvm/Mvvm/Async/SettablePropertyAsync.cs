#nullable enable

using LionFire.Resolves;
using LionFire.Results;
using MorseCode.ITask;

namespace LionFire.Mvvm.Async;

public class SettablePropertyAsync<TObject, TValue>
    : PropertyAsync<TObject, TValue>
    , IObservableSets<TValue>
{
    #region Lifecycle

    public SettablePropertyAsync(TObject target, AsyncPropertyOptions? options = null) : base(target, options)
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
    /// locked by: SyncRoot
    /// </remarks>
    public (Task? task, TValue? SettingToValue) SetState
    {
        get => setState;
        private set
        {
            // REVIEW - is this necessary?
            //bool oldIsSetting = IsSetting;
            //setState = value;

            this.RaiseAndSetIfChanged(ref setState, value);

            //if (IsSetting != oldIsSetting)
            //{
            //    OnPropertyChanged(nameof(IsSetting));
            //}
        }
    }
    private (Task? task, TValue? SettingToValue) setState;

    #endregion

    #region Set

    private object setLock = new();

    public Func<TObject, TValue?, CancellationToken, Task> Setter { get; set; }
    public async Task Set(TValue? value, CancellationToken cancellationToken = default)
    {
        (Task? task, TValue? SettingToValue) currentState;
    start:
        do
        {
            currentState = SetState;
            if (currentState.task != null) { await currentState.task!.ConfigureAwait(false); }
            if (EqualityComparer.Equals(currentState.SettingToValue, value)) { return; }
        } while (currentState.task != null);

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


    public IObservable<Task<TValue>> Sets => sets;
    private Subject<Task<TValue>> sets = new();

    #endregion

    #region IObservableSets

    IObservable<ITask<ISuccessResult>> IObservableSets<TValue>.Sets => throw new NotImplementedException();

    Task<ISuccessResult> ISets<TValue>.Set(TValue value, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    #endregion
}

