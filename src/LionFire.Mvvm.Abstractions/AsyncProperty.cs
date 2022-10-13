#nullable enable

using System.Runtime.CompilerServices;

namespace LionFire.Mvvm;

public class AsyncProperty<TObject, TProperty> : INotifyPropertyChanged
{
    // TODO: Wire up change notifications from source
    private object SyncRoot = new();
    public IEqualityComparer<TProperty> EqualityComparer { get; set; } = EqualityComparer<TProperty>.Default;


    public TObject Target { get; }
    public AsyncPropertyOptions Options { get; }
    public static AsyncPropertyOptions DefaultOptions = new();
    public AsyncProperty(TObject target, AsyncPropertyOptions? options = null)
    {
        Target = target;
        Options = options ?? DefaultOptions;
    }

    public bool HasValue { get; private set; }
    public bool IsGetting { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// locked by: SyncRoot
    /// </remarks>
    public (Task? task, TProperty? SettingToValue) SetState
    {
        get => setState;
        private set
        {
            bool oldIsSetting = IsSetting;
            setState = value;

            if (IsSetting != oldIsSetting)
            {
                OnPropertyChanged(nameof(IsSetting));
            }
        }
    }
    private (Task? task, TProperty? SettingToValue) setState;

    public bool IsSetting => SetState.task != null;

    public TProperty Value
    {
        get
        {
            if (!HasValue)
            {
                if (Options.GetOnDemand)
                {
                    if (Options.BlockToGet)
                    {
                        return Get().Result;
                    }
                    else
                    {
                        Get().FireAndForget();
                    }
                }
                else if (Options.ThrowOnGetValueIfNotLoaded) { DoThrowOnGetValueIfNotLoaded(); }
            }
            return cachedValue;
        }
        set
        {
            cachedValue = value;
        }
    }
    private TProperty cachedValue;

    private void DoThrowOnGetValueIfNotLoaded() => throw new Exception("Value has not been gotten yet.  Invoke Get first or disable Options.ThrowOnGetValueIfNotLoaded");

    public async Task<TProperty?> Get(CancellationToken cancellationToken = default)
    {
        var setState = SetState;
        if (IsSetting && Options.OptimisticGetWhileSetting) { return setState.SettingToValue; } // return Optimistically

        IsGetting = true;
        try
        {
            var result = await Getter(Target, cancellationToken).ConfigureAwait(false);
            Value = result;
            return result;
        }
        finally
        {
            IsGetting = false;
        }
    }

    public async Task Set(TProperty? value, CancellationToken cancellationToken = default)
    {
        (Task? task, TProperty? SettingToValue) currentState;
    start:
        do
        {
            currentState = SetState;
            if (currentState.task != null) { await currentState.task!.ConfigureAwait(false); }
            if (EqualityComparer.Equals(currentState.SettingToValue, value)) { return; }
        } while (currentState.task != null);

        try
        {
            lock (SyncRoot)
            {
                if (IsSetting) goto start;
                SetState = (Setter(Target, value, cancellationToken), value);
            }
            await SetState.task.ConfigureAwait(false);
            OnPropertyChanged(nameof(Value));
        }
        finally
        {
            SetState = (null, default);
        }
    }

    public Func<TObject, CancellationToken, Task<TProperty>> Getter { get; set; }
    public Func<TObject, TProperty?, CancellationToken, Task> Setter { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;


    #region INotifyPropertyChanged Implementation


    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    #endregion

}

