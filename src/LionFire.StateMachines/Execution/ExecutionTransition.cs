using LionFire.StateMachines;
using System;

namespace LionFire.Execution
{

    [Flags]
    public enum ExecutionTransition
    {
        [Transition(TState.Uninitialized, TState.Ready)]
        Initialize = 1 << 1,

        [Transition(TState.Uninitialized, TState.Finished)]
        Invalidate = 1 << 2,

        [Transition(TState.Ready, TState.Uninitialized)]
        Deinitialize = 1 << 3,

        [Transition(TState.Ready, TState.Running)]
        Start = 1 << 4,

        [Transition(TState.Ready, TState.Finished)]
        Skip = 1 << 5,

        [Transition(TState.Ready, TState.Finished)]
        Noop = 1 << 6,

        [Transition(TState.Running, TState.Ready)]
        Undo = 1 << 7,
        [Transition(TState.Running, TState.Finished)]
        Complete = 1 << 8,
        [Transition(TState.Running, TState.Finished)]
        Terminate = 1 << 9,
        [Transition(TState.Running, TState.Finished)]
        Fail = 1 << 10,

        [Transition(TState.Finished, TState.Disposed)]
        CleanUp = 1 << 12,
        [Transition(TState.Finished, TState.Ready)]
        Reset = 1 << 13,
        [Transition(TState.Finished, TState.Uninitialized)]
        Reuse = 1 << 14,

        Cancel = Deinitialize | Undo,
        End = Complete | Skip,
        Fault = Invalidate | Fail,
        Dispose = CleanUp,

    }
}
