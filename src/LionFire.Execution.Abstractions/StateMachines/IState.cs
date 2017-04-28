#if truex
using LionFire.Structures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LionFire.Execution
{
    public interface IState
    {
    }
    public interface IState<T> : IState
        where T : class
    {
        T StateData { get; set; }
    }
    public class State<T> : IState<T>
    {
        public T StateData { get; set; }

        public List<object> Participants { get; set; }
    }

    public interface IStateParticipant
    {

    }

    public class StateParticipant<T> : IStateParticipant
        where T:class
    {
        public IState<T> State { get; set; }

        public Type Type { get; set; }

        public virtual void OnEnter()
        {
        }

        public virtual  (bool canLeave, object reason) CanLeave()
        {
            return (true, null);
        }

    }

    public static class StateExtensions
    {


    }

    public class ServiceCollectionPopulation : State<IServiceCollection>, IHasDescription
    {
        public string Description => "All ServiceDescriptions are provided to an AppHost's ServicesCollection at application startup";
    }

    public class StateMachine
    {

        #region CurrentState

        public IState CurrentState
        {
            get { return currentState; }
            private set
            {
                if (currentState == value) return;
                currentState = value;

            }
        }
        private string currentState;

        #endregion


        public void OnStateTransition(IState from, IState to)
        {
            StateChanged?.Invoke(this, from, to);
        }
        public Action<StateMachine, IState, IState> StateChanged;
    }

    public class StateMachineSequence
    {
        public List<IState> States { get; set; } = new List<IState>();

        public void Add<T>()
            where T : IState
        {
            States.Add(Activator.CreateInstance<T>());
        }

        public static StateMachineSequence AppHostStateMachine
        {
            get
            {
                var result = new StateMachineSequence();

                result.Add<ServiceCollectionPopulator>();
                result.Add<InjectDependencies>();
            }
        }

        public void Run()
        {

        }
    }
    

    public class ServiceCollectionPopulator : StateParticipant<IServiceCollection>
    {
        public override void OnEnter()
        {
            base.OnEnter();

            State.StateData.AddSingleton(typeof(string), "The one string to rule them all");


        }
    }

    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class InjectPropertiesAttribute : Attribute
    {
        public InjectPropertiesAttribute() { }
    }


    public class InjectDependencies : State<IEnumerable<object>>
    {
        public bool ShouldIncludeParticipant(object obj)
        {
            var attr = obj.GetType().GetTypeInfo().GetCustomAttribute<InjectPropertiesAttribute>();
            if (attr != null) return true;
            return false;
        }
    }
}
#endif