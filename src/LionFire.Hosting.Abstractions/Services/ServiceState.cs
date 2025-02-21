//using LionFire.Data.Async;
namespace LionFire.Hosting.Services;

[Flags]
public enum ServiceState
{
    Unspecified = 0,
    //Uninitialized = 1 << 0,
    //Initialized = 1 << 1,

    Stopped = 1 << 5,

    Starting = 1 << 2,

    Running = 1 << 3,

    Pausing = 1 << 8,
    Paused = 1 << 9,
    Resuming = 1 << 10,
    Resumed = 1 << 11,

    Stopping = 1 << 4,

    Disposed = 1 << 6,

    //Faulted = 1 << 7,
    //Restarting = 1 << 12,
}
