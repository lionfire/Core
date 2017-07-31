using LionFire.StateMachines.Class;
using System;

namespace LionFire.Execution
{

    [Flags]
    public enum ExecutionTransition
    {
        [Transition(ExecutionState.Uninitialized, ExecutionState.Ready)]
        Initialize = 1 << 1,

        [Transition(ExecutionState.Uninitialized, ExecutionState.Finished)]
        Invalidate = 1 << 2,

        [Transition(ExecutionState.Ready, ExecutionState.Uninitialized)]
        Deinitialize = 1 << 3,

        [Transition(ExecutionState.Ready, ExecutionState.Running)]
        Start = 1 << 4,

        [Transition(ExecutionState.Ready, ExecutionState.Finished)]
        Skip = 1 << 5,

        [Transition(ExecutionState.Ready, ExecutionState.Finished)]
        Noop = 1 << 6,

        [Transition(ExecutionState.Running, ExecutionState.Ready)]
        Undo = 1 << 7,
        [Transition(ExecutionState.Running, ExecutionState.Finished)]
        Complete = 1 << 8,
        [Transition(ExecutionState.Running, ExecutionState.Finished)]
        Terminate = 1 << 9,
        [Transition(ExecutionState.Running, ExecutionState.Finished)]
        Fail = 1 << 10,

        [Transition(ExecutionState.Finished, ExecutionState.Disposed)]
        CleanUp = 1 << 12,
        [Transition(ExecutionState.Finished, ExecutionState.Ready)]
        Reset = 1 << 13,
        [Transition(ExecutionState.Finished, ExecutionState.Uninitialized)]
        Reuse = 1 << 14,

        Cancel = Deinitialize | Undo,
        End = Complete | Skip,
        Fault = Invalidate | Fail,
        Dispose = CleanUp,

    }
}
