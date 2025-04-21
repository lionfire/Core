using DynamicData;
using DynamicData.Kernel;
using LionFire.FlexObjects;

namespace LionFire.Workspaces.Services;

public abstract class Runner<TValue, TRunner> : IRunner<TValue>, IDisposable
    where TValue : notnull
    where TRunner : IRunner<TValue>
{
    static bool IRunner<TValue>.IsEnabled(TValue value) => throw new NotImplementedException();

    public Predicate<TValue> EnabledPredicate { get; }
    Optional<TValue> value;

    #region Lifecycle

    //public Runner(Predicate<TValue> enabledPredicate)
    //{
    //    EnabledPredicate = enabledPredicate;
    //}
    public Runner()
    {
        //EnabledPredicate = (Predicate<Optional<TValue>>)(o => o.HasValue && IsEnabled(o.Value));
        //EnabledPredicate = (Predicate<TValue>)(o =>  IsEnabled(o));
        //EnabledPredicate = o => IsEnabled(o);
        EnabledPredicate = TRunner.IsEnabled;
    }

    #endregion

    #region State

    public WorkerStatus WorkerStatus { get; } = new();

    #endregion

    public void Dispose() { }

    #region IObserver

    public void OnCompleted() => Dispose();

    public void OnError(Exception error)
    {
        WorkerStatus.faults.Add(error);
        //Logger.LogError(error, "Runner error");
    }

    public void OnNext(TValue newValue)
    {
        var oldValue = value;
        value = newValue;
        OnValueChanged(newValue, oldValue);
    }

    #endregion

    private bool IsEnabled(Optional<TValue> optional) => optional.HasValue && EnabledPredicate(optional.Value);

    protected Optional<TValue> Current;
    protected void OnValueChanged(Optional<TValue> newValue, Optional<TValue> oldValue)
    {
        var wasEnabled = IsEnabled(oldValue);
        var isEnabled = IsEnabled(newValue);

        var oldConfiguration = Current;
        Current = newValue;

        if (isEnabled && newValue.HasValue)
        {
            if (wasEnabled)
            {
                OnConfigurationChange(newValue.Value, oldValue);
            }
            else
            {
                Start(newValue.Value, oldValue);
            }
        }
        else
        {
            if (oldConfiguration.HasValue && wasEnabled)
            {
                Stop(newValue.Value, oldConfiguration.Value);
            }
        }
    }

    #region TODO - move OnValueChanged conditions into these:

    protected abstract void Start(TValue value, Optional<TValue> oldValue);
    protected abstract void OnConfigurationChange(TValue newValue, Optional<TValue> oldValue);
    protected abstract void Stop(TValue newValue, TValue oldValue);

    #endregion

}
