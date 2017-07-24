using System.Threading;

namespace LionFire.StateMachines
{

    public class StateMachineState<TState, TTransition> : IStateMachineState<TState, TTransition>
    {

        public StateMachine<TState, TTransition> template;

        #region CurrentState

        public TState CurrentState
        {
            get { return currentState; }
            private set { currentState = value; }
        }
        private TState currentState;

        #endregion

        internal bool ChangeCurrentState(StateTransitionInfo<TState ,TTransition> transition)
        {
            if (!currentState.Equals(transition.From)) return false;
            currentState = transition.To;
            return true;
        }

        public StateChange<TState, TTransition> LastTransition { get; set; }

    }

#if FUTUREOPTIMIZATION
    public class IntStateMachineState<TState, TTransition> : IStateMachineState<TState, TTransition>
    {

#region CurrentState

        public TState CurrentState
        {
            get { return (TState)(object)currentState; }
            set { currentState = (int)(object)value; }
        }
        private int currentState;

#endregion

        internal bool TransitionFromTo(TState from, TState to)
        {
            var fromI = (int)(object)from;
            var toI = (int)(object)to;
            return Interlocked.CompareExchange(ref currentState, toI, fromI) == toI;
        }

        public StateChange<TState, TTransition> LastTransition { get; set; }

    }


#endif
}
