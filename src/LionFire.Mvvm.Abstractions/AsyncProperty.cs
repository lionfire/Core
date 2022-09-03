#nullable enable

namespace LionFire.UI;

public class AsyncProperty<TProperty> : INotifyPropertyChanged
{
    // TODO: Wire up change notifications from source


    public object Target { get; }
    public AsyncPropertyOptions Options { get; }

    public AsyncProperty(object target, AsyncPropertyOptions options)
    {
        Target = target;
        Options = options;
    }

    public bool HasValue { get; private set; }
    public bool IsGetting { get; private set; }
    public (bool IsSetting, TProperty? SettingToValue) SetState { get; private set; }

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

    public async Task<TProperty> Get(CancellationToken cancellationToken = default)
    {
        var setState = SetState;
        if (setState.IsSetting && Options.OptimisticGetWhileSetting) { return setState.SettingToValue; }

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
        SetState = (true, value);
        try
        {
            await Setter(Target, value, cancellationToken);
        }
        finally
        {
            SetState = (false, default);
        }
    }

    public Func<object, CancellationToken, Task<TProperty>> Getter { get; set; }
    public Func<object, TProperty, CancellationToken, Task> Setter { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}

