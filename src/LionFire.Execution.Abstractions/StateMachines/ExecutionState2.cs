using LionFire.StateMachines.Class;
using System;

namespace LionFire.Execution
{
    // RENAME to ExecutionState or maybe SimpleExecutionState
    [Flags]
    public enum ExecutionState2 : int
    {
        //[Start]
        Uninitialized = 1 << 1,
        Ready = 1 << 2,
        Running = 1 << 3,
        Finished = 1 << 4,
        //[End]
        Disposed = 1 << 5,
    }

    // OLD - REVIEW and discard
    //public class Ready : StateBase
    //{
    //    public override IEnumerable<Type> AllowsTransitionTo { get { yield return typeof(Uninitialized); } }
    //    public override IEnumerable<Type> AllowsTransitionFrom { get { yield return typeof(Uninitialized); } }
    //}


}
