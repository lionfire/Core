namespace LionFire.Execution;

public interface IRunner<TValue> : IObserver<TValue>
    where TValue : notnull
{
    //IObservable<bool> Enabled { get; }
    //IObservableList<object>? Faults { get; }

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
    Uninitialized = 1 << 1,
    Invalid = 1 << 2,
    Ready = 1 << 3,

    Disabled = 1 << 4,
    StartPending = 1 << 5,
    Starting = 1 << 6,

    Running = 1 << 7,

    StopPending = 1 << 8,
    Stopping = 1 << 9,


    PausePending = 1 << 12,
    Pausing = 1 << 13,

    ResumePending = 1 << 14,
    Resuming = 1 << 15,
    

    Faulted = 1 << 30,
    Disposed = 1 << 31,
}
