using System;
using TS = LionFire.Execution.ExecutionState2;
using LionFire.StateMachines;

namespace LionFire.Execution;


[Flags]
public enum ExecutionTransition
{

    // Construction

    [Transition(null, TS.Uninitialized)]
    Create = 1 << 0,


    // Initialization

    [Initialize]
    [Transition(TS.Uninitialized, TS.Ready)]
    [TransitionKind(TransitionKind.Initialize)]
    Initialize = 1 << 1,

    [Transition(TS.Uninitialized, TS.Finished)]
    Invalidate = 1 << 2,

    [Transition(TS.Ready, TS.Uninitialized)]
    Deinitialize = 1 << 3,


    // Running

    [Transition(TS.Ready, TS.Running)]
    [TransitionKind(TransitionKind.Run)]
    Start = 1 << 4,

    [Transition(TS.Ready, TS.Finished)]
    Skip = 1 << 5,

    [Transition(TS.Ready, TS.Finished)]
    Noop = 1 << 6,

    [Transition(TS.Running, TS.Ready)]
    Undo = 1 << 7,

    [Transition(TS.Running, TS.Finished)]
    [TransitionKind(TransitionKind.Wait)]
    Complete = 1 << 8,

    [Transition(TS.Running, TS.Finished)]
    Terminate = 1 << 9,

    [Transition(TS.Running, TS.Finished)]
    Fail = 1 << 10,

    // Cleanup

    [Transition(TS.Finished, TS.Disposed)]
    [TransitionKind(TransitionKind.Cleanup)]
    CleanUp = 1 << 12,
    [Transition(TS.Finished, TS.Ready)]

    Reset = 1 << 13,
    [Transition(TS.Finished, TS.Uninitialized)]

    Reuse = 1 << 14,

    [Transition(TS.Disposed, null)]
    [TransitionKind(TransitionKind.Cleanup)]
    Destroy = 1 << 15,

    // Aggregates

    Cancel = Deinitialize | Undo,
    End = Complete | Skip,
    Fault = Invalidate | Fail,
    Dispose = CleanUp,

}
