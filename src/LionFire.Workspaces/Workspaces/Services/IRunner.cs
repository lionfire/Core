namespace LionFire.Workspaces.Services;

public interface IRunner<TValue> : IObserver<TValue>
    where TValue : notnull
{
    //IObservable<bool> Enabled { get; }
    //IObservableList<object>? Faults { get; }

    //bool IsEnabled(TValue value);
    static abstract bool IsEnabled(TValue value);
}

public interface IEnablable
{
    bool Enabled { get; }
}


[Flags]
public enum RunnerState
{
    Unspecified = 0,
    Disabled = 1 << 0,
    StartPending = 1 << 1,
    Starting = 1 << 2,
    Running = 1 << 3,
    StopPending = 1 << 4,
    Stopping = 1 << 5,
    Disposed = 1 << 16,

}
