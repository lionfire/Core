using LionFire.StateMachines;

namespace LionFire.Execution
{
    //public interface IStateful<TState,TTransition>
    //{
    //    IStateMachine<TState, TTransition> StateMachine { get; set; }
    //}


    //public static class IStatefulExtensions
    // {
    //     public static TState CurrentState<TState, TTransition>(this IStateful<TState,TTransition> stateful)
    //     {
    //         return stateful.StateMachine.State;
    //     }
    // }

    //public interface ITransitions<TTransition>
    //{
    //    event Action<TTransition, ITransitions<TTransition>> TransitionStarting;
    //    event Action<TTransition, ITransitions<TTransition>> TransitionCompleted;
    //    event Action<TTransition, ITransitions<TTransition>> TransitionAborted;
    //}

    //public interface IHasStateMachine<TState, TTransition>
    //{
    //    IStateMachine<TState,TTransition>
    //}

    //public interface IStateMachine<TState,TTransition> : ITransitions<TTransition>, IStateful<TState>
    //{
    //}

    public interface IExecutableStateMachine : IStateMachine<ExecutionState2, ExecutionTransition> { }

}
