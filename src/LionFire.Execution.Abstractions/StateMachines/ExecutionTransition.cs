using LionFire.StateMachines.Class;
using System;
using TS = LionFire.Execution.ExecutionState2;
using LionFire.StateMachines;

namespace LionFire.Execution
{

    [Flags]
    public enum ExecutionTransition
    {
        [Run]
        [Transition(null, TS.Uninitialized)]
        Create = 1 << 0,

        [Run]
        [Transition(TS.Uninitialized, TS.Ready)]
        Initialize = 1 << 1,

        [Transition(TS.Uninitialized, TS.Finished)]
        Invalidate = 1 << 2,

        [Transition(TS.Ready, TS.Uninitialized)]
        Deinitialize = 1 << 3,

        [Run]
        [Transition(TS.Ready, TS.Running)]
        Start = 1 << 4,

        [Transition(TS.Ready, TS.Finished)]
        Skip = 1 << 5,

        [Transition(TS.Ready, TS.Finished)]
        Noop = 1 << 6,

        [Transition(TS.Running, TS.Ready)]
        Undo = 1 << 7,

        [Run]
        [Transition(TS.Running, TS.Finished)]
        Complete = 1 << 8,

        [Transition(TS.Running, TS.Finished)]
        Terminate = 1 << 9,
        [Transition(TS.Running, TS.Finished)]
        Fail = 1 << 10,

        [CleanUp]
        [Transition(TS.Finished, TS.Disposed)]
        CleanUp = 1 << 12,
        [Transition(TS.Finished, TS.Ready)]
        Reset = 1 << 13,
        [Transition(TS.Finished, TS.Uninitialized)]
        Reuse = 1 << 14,

        [CleanUp]
        [Transition(TS.Disposed, null)]
        Destroy = 1 << 15,

        Cancel = Deinitialize | Undo,
        End = Complete | Skip,
        Fault = Invalidate | Fail,
        Dispose = CleanUp,

    }
}
