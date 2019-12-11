using System.Collections.Generic;
using System.Linq;

namespace LionFire.Execution.Hosts
{
    //public class TestExecutable : ExecutableBase
    //{
    //    private void StateMachine_StateChanging(StateMachines.IStateChange<ExecutionState2, ExecutionTransition> context)
    //    {
    //        //Debug.WriteLine(StateMachine_StateChanging);
    //    }
    //}

    //public class ExecutableHost : ExecutableBase, IExecutable2
    //{

    //    public IExecutable2 Child
    //    {
    //        get
    //        {
    //            return child;
    //        }
    //        set
    //        {
    //            if (child == value) return;
    //            if (StateMachine.CurrentState != ExecutionState2.Uninitialized) throw new InvalidExecutionStateException("Set child", ExecutionStateEx.Uninitialized, StateMachine.CurrentState);
    //            child = value;
    //        }
    //    }
    //    private IExecutable2 child;

    //    public Func<ValidationContext> vc => () => new ValidationContext(this);

    //    public object CanInitialize
    //    {
    //        get {
    //            var v = Validate.For(this).PropertyNotNull(nameof(Child), Child))); if (!v.Valid) return v.Issues;
    //            var result = Child.CanInitialize
    //        }

    //        return null;
    //    }

    //}

    public class ExecutionGoal
    {
        public IExecutable2 Executable { get; set; }
        public ExecutionState2 DesiredState { get; set; }
        public List<object> FailureReasons { get; set; }

        public bool IsFaulted => FailureReasons != null && FailureReasons.Any();
    }
}

