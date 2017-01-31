using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public enum ExecutionStateFlags
    {
        None = 0,
        Faulted = 1 << 1,
        InvalidConfiguration = 1 << 2,
        Autostart = 1 << 3,
        WaitingToStart = 1 << 13,
        Restarting = 1 << 14,
    }

    public interface IHasExecutionStateFlags
    {
        ExecutionStateFlags ExecutionStateFlags { get; }
    }
    public interface IAcceptsExecutionStateFlags : IHasExecutionStateFlags
    {
        new ExecutionStateFlags ExecutionStateFlags { get; set; }
    }
}
