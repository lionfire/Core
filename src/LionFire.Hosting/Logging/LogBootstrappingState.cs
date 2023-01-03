#nullable enable
using System;
using System.Threading;

namespace LionFire.Hosting;

public class LogBootstrappingState
{
    public static bool FileLogDuringBootstrap { get; set; }

    public static AsyncLocal<LogBootstrappingState> State = new();

    public bool HasLoggedStart { get; set; }
    public static bool IsBootstrapping
    {
        get
        {
            return State.Value != null;
        }
        set
        {
            if (value == IsBootstrapping) return;
            if (value)
            {
                State.Value = new LogBootstrappingState();
            }
            else
            {
                State.Value = null;
            }
        }
    }
}
