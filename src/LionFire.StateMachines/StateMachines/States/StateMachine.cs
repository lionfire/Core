
using System.Reflection;
using LionFire.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ES = LionFire.Execution.ExecutionState;
using ET = LionFire.Execution.ExecutionTransition;

// TODO: Once this gets fleshed out, move to Core? Or create shared library?

namespace LionFire.StateMachines
{

    //public interface IState
    //{
    //    string Name { get; }
    //}

    //public abstract class StateBase : IState
    //{
    //    public string Name { get; private set; }
    //    public StateBase(string name) { this.Name = name; }
    //    //    public abstract IEnumerable<Type> AllowsTransitionTo { get; }
    //    //    public abstract IEnumerable<Type> AllowsTransitionFrom { get; }
    //}
    //public class State
    //{
    //    public State(string name) : base(name) { }

    //    public static implicit operator State(string name) { return new State(name); }
    //}
    //public class Uninitialized : StateBase
    //{
    //}

    //public interface IHasStateMachine<TState,TTransition>
    //{
    //    IStateMachine<TState,TTransition> StateMachine { get; }
    //}
    //public interface IHasStateMachine
    //{
    //    IStateMachine StateMachine { get; }
    //}
    //public static class IHasStateMachineExtensions
    //{
    //    public static TState CurrentState<TState, TTransition>(this IHasStateMachine<TState, TTransition> sm)
    //    {
    //        return default(TState);//STUB
    //    }
    //}

    public interface IStateModel { }
    public interface IStateModel<S, T> : IStateModel {
        S States { get; }
        T Transitions { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TTransitions">An enum consisting of flags for states</typeparam>
    public class StateMachine<TStates,TTransitions>: IStateModel<TStates,TTransitions>
    {

        public TStates States { get; private set; }
        public TTransitions Transitions { get; private set; }

        Dictionary<TTransitions, StateTransition> transitions = new Dictionary<TTransitions, StateTransition>();
        //Dictionary<TStates, IState> states = new Dictionary<TStates, IState>();

        public StateMachine()
        {
        }
        public StateMachine<TStates, TTransitions> AddTransition(TStates from, TStates to, TTransitions transition = default(TTransitions))
        {
            ////if (transition == null) transition = $"{from} -> {to}";
            //var t = new StateTransition(transition);
            //transitions.Add(t.Key, t);
            return this;
        }

        public StateMachine<TStates, TTransitions> AddTransition(string from, string to, string transitionName = null)
        {
            return AddTransition((TStates)Enum.Parse(typeof(TStates), from), (TStates)Enum.Parse(typeof(TStates), to), (TTransitions)Enum.Parse(typeof(TTransitions), transitionName));
        }

        public void ChangeState(TTransitions transition)
        {
            
        }

        public StateMachine<TStates, TTransitions> TrimForClass<TStateOwner>()
        {
            //var mis = typeof(TStateOwner).GetRuntimeMethods();

            Type t = typeof(TStateOwner);
            var mis = t.GetMethods();
            // see if mis contains OnTransition or OnState or AfterState methods.  Either no param, or object param.

            //TStates 

            //foreach(var

            //var trimmed = new StateModel<TStates, TTransitions>();

            return this;
        }
    }

    public class ExecutionStateModel : StateMachine<ES, ET>, IExecutionStateModel
    {
    }

    public interface IExecutionStateModel : IStateModel<ExecutionState, ExecutionTransition> { }
    public class TExecutableStateMachine
    {
        //public static StateModel<ExecutionState, ExecutionTransition>
        public static IExecutionStateModel Create(ES states = default(ES), ET transitions = default(ET))
        {
            
            var sm = new ExecutionStateModel()
                    .AddTransition(ES.Uninitialized, ES.Ready, ET.Initialize)
                    .AddTransition("Uninitialized", "Finished", "Fault.Invalidate")

                    .AddTransition("Ready", "Uninitialized", "Cancel.Deinitialize")
                    .AddTransition("Ready", "Running", "Start")
                    .AddTransition("Ready", "Finished", "Complete.Noop")
                    .AddTransition("Ready", "Finished", "Abort.Skip")

                    .AddTransition("Running", "Ready", "Cancel.Undo")
                    .AddTransition("Running", "Finished", "Complete.Finish")
                    .AddTransition("Running", "Finished", "Abort.Terminate")
                    .AddTransition("Running", "Finished", "Fault.Fail")

                    // Sub states: PauseMachine
                    //.AddTransition("Running", "Paused", "Pause")
                    //.AddTransition("Paused", "Running", "Unpause")

                    // Sub states: ProcessorMachine
                    // Waiting
                    // Processing

                    .AddTransition("Finished", "Ready", "Reset")
                    .AddTransition("Finished", "Uninitialized", "Deinitialize.Reuse")
                    .AddTransition("Uninitialized", "Ready", "Initialize") 
                    ;
                return (ExecutionStateModel)sm;
        }
}

public interface ITransitionComponent<TTargetState>
{
    bool CanTransition(Type from, Type to, object context);
    Task Transition(Type from, Type to, object context = null);
}
public static class IStateMachineExtensions
{
    //public static Task Transition(this IStateMachine sm, Type to, object context = null) { return sm.Transition(null, to, context); }
    //public static Task Transition<TFrom, TTo>(this IStateMachine sm, object context = null) { return sm.Transition(typeof(TFrom), typeof(TTo), context); }
}

public class StateTransitionException : Exception
{
    public StateTransitionException() { }
    public StateTransitionException(string message) : base(message) { }
    public StateTransitionException(string message, Exception inner) : base(message, inner) { }
}
}
namespace LionFire.StateMachines
{
    //public static class StateChanger
    //{
    //    public static Task<StateChangeContext> ToState(this object obj, object state)
    //    {
    //        //var c = new StateChangeContext();

    //    }
    //}

    public class StateChangeContext
    {
    }

    //public struct ValidatingStateChangeContext
    //{
    //    public ValidationContext ValidationContext { get; set; }
    //}

}
