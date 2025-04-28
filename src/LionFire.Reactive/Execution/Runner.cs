using DynamicData;
using DynamicData.Kernel;
using LionFire.FlexObjects;
using System.Reactive.Subjects;

namespace LionFire.Execution;

public abstract class Runner<TValue, TRunner>
    : IRunner<TValue>
    , IDisposable
    where TValue : notnull
    where TRunner : IRunner<TValue>
{
    #region (static)

    /// <summary>
    /// Implement this on TRunner, and have TRunner directly implement (i.e. inherit from) IRunner<TValue>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    static bool IRunner<TValue>.IsEnabled(TValue value) => throw new NotImplementedException();

    #endregion

    #region Dependencies

    public Predicate<TValue> EnabledPredicate { get; }

    #endregion

    #region Lifecycle

    public Runner()
    {
        EnabledPredicate = TRunner.IsEnabled;
    }

    public void Dispose() { }

    #endregion

    #region State

    #region Current Value

    protected Optional<TValue> Current { get; set; }

    #region Derived

    private bool IsEnabled(Optional<TValue> optional) => optional.HasValue && EnabledPredicate(optional.Value);

    #endregion

    #endregion

    public WorkerStatus WorkerStatus { get; } = new();

    #endregion

    #region IObservable

    public IObservable<Exception> Errors => errors ??= new Subject<Exception>();
    protected Subject<Exception>? errors;

    #endregion

    #region IObserver

    public void OnNext(TValue newValue)
    {
        var oldValue = Current;
        Current = newValue;
        //OnValueChanged(newValue, oldValue);

        var wasEnabled = IsEnabled(oldValue);
        var isEnabled = IsEnabled(newValue);

        var oldConfiguration = Current;
        Current = newValue;

        if (isEnabled)
        {
            if (wasEnabled)
            {
                OnConfigurationChange(newValue, oldValue);
            }
            else
            {
                try
                {
                    WorkerStatus.RunnerState = RunnerState.Starting;
                    WorkerStatus.faults.RemoveKey(Runner_StartException);
                    Start(newValue, oldValue);
                    WorkerStatus.RunnerState = RunnerState.Running;
                }
                catch (Exception ex)
                {
                    WorkerStatus.faults.AddOrUpdate((Runner_StartException, ex));
                    WorkerStatus.RunnerState = RunnerState.Faulted;
                }
            }
        }
        else
        {
            if (oldConfiguration.HasValue && wasEnabled)
            {
                Stop(newValue, oldConfiguration.Value);
            }
        }
    }
    public const string Runner_StartException = "Runner_StartException";

    public void OnCompleted()
    {
        Stop(Optional<TValue>.None, Current);
        Dispose();
    }


    public void OnError(Exception error)
    {
        errors?.OnNext(error);
        //WorkerStatus.faults.Add(error);
        //Logger.LogError(error, "Runner error");
    }

    #endregion

    #region (abstract)

    protected abstract ValueTask<bool> Start(TValue value, Optional<TValue> oldValue);
    protected abstract void OnConfigurationChange(TValue newValue, Optional<TValue> oldValue);
    protected abstract void Stop(Optional<TValue> newValue, Optional<TValue> oldValue);

    #endregion

}

