using LionFire.StateMachines.Class;
using System;

namespace LionFire.Execution
{
    [Flags]
    public enum ExecutionState2 : int
    {
        [Start]
        Uninitialized = 1 << 0,
        Ready = 1 << 1,
        Running = 1 << 2,
        Finished = 1 << 3,
        [End]
        Disposed = 1 << 4,
    }
    //public class Ready : StateBase
    //{
    //    public override IEnumerable<Type> AllowsTransitionTo { get { yield return typeof(Uninitialized); } }

    //    public override IEnumerable<Type> AllowsTransitionFrom { get { yield return typeof(Uninitialized); } }
    //}

    //public class Running : StateBase
    //{
    //    public override IEnumerable<Type> AllowsTransitionFrom { get { yield return typeof(Paused); yield return typeof(Finished); } }
    //    public override IEnumerable<Type> AllowsTransitionTo { get { yield return typeof(Paused); yield return typeof(Finished); } }
    //}
    //public class Paused : StateBase
    //{
    //    public override IEnumerable<Type> AllowsTransitionTo { get { yield return typeof(Running); yield return typeof(Finished); } }

    //    public override IEnumerable<Type> AllowsTransitionFrom { get { yield return typeof(Running); } }
    //}
    //public class Finished : StateBase
    //{
    //    public override IEnumerable<Type> AllowsTransitionFrom { get { yield return typeof(Running); yield return typeof(Finished); } }

    //    public override IEnumerable<Type> AllowsTransitionTo { get { yield return typeof(Disposed); yield return typeof(Uninitialized); } }

    //}
    //public class Disposed : StateBase
    //{
    //}
}
