//using LionFire.Reactive;
//using LionFire.Reactive.Subjects;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace LionFire.StateMachines
//{
//    // TODO: Review Behaviors library

//    public interface IStateMachine 
//    {
//        IBehaviorObservable<State> State { get; }
        

//    }
//    public class StateMachine : IStateMachine
//    {
//        public IBehaviorObservable<State> State { get { return state; } }
//        BehaviorObservable<State> state = new BehaviorObservable<StateMachines.State>();

//        public State DesiredState { get; set; }

//    }

//    public class StateSequence
//    {
//        public IStateMachine StateMachine { get; set; }
//        public Queue<State> DesiredStateSequence { get; } = new Queue<State>();
//        // TODO
//    }

//    public class State
//    {
//        // [SetOnce]
//        public string Name { get; set; }


//    }
//}
