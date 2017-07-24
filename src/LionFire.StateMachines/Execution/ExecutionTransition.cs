using System;

namespace LionFire.Execution
{
    [Flags]
    public enum ExecutionTransition
    {
        Initialize = 1 << 1,
        Invalidate = 1 << 2,

        Deinitialize = 1 << 3,
        Start = 1 << 4,
        Skip = 1 << 5,
        Noop = 1 << 6,

        Undo = 1 << 7,
        Finish = 1 << 8,
        Terminate = 1 << 9,
        Fail = 1 << 10,

        Cancel = Deinitialize | Undo,
        Complete,
        Fault = Invalidate | Fail,

    }
}
