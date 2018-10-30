using LionFire.StateMachines;
using HSM = LionFire.Ontology.IHas<LionFire.StateMachines.IStateMachine<LionFire.Execution.ExecutionState2, LionFire.Execution.ExecutionTransition>>;

namespace LionFire.Execution
{
    public interface IExecutable2 : HSM
    {
    }

    public static class IExecutable2Extensions
    {
        public static IStateMachine<ExecutionState2, ExecutionTransition> StateMachine(this IExecutable2 exe)
        {
            return ((HSM)exe).Object;
        }
        public static void Transition(this IExecutable2 exe, ExecutionTransition transition)
        {
            ((HSM)exe).Object.Transition(transition);
        }

        public static void TransitionToState(this IExecutable2 exe, ExecutionState2 state)
        {
            // TODO: Get task for current transition in progress.  If it matches, just wait on it.  If there's no task but it's in progress, wait for the event to come.
            // TODO: IAsyncStateMachine: add "Task" for task in progress
            // TODO: IStateMachine: add "Transition" for transition in progress.
        }
    }

}
