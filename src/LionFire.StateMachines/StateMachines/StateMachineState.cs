using System;
using System.Threading;

namespace LionFire.StateMachines
{

    public class StateMachineState<TState, TTransition,TOwner> : IStateMachineState<TState, TTransition,TOwner>
    {
        public TOwner Owner { get; private set; }
        public StateChange<TState, TTransition,TOwner> LastTransition { get; set; }


        public StateMachineState(TOwner owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            this.Owner = owner;
            currentState = StateMachine<TState, TTransition>.StartingState;
        }

        #region CurrentState

        public TState CurrentState
        {
            get { return currentState; }
            private set { currentState = value; }
        }
        private TState currentState;

        #endregion

        // TODO: Also offer TryChangeState 
        // FUTURE: goal seeking adapter

        //public void ChangeState(TTransition transition, object transitionData = null)
        //{
        //    ChangeState(stateMachine.GetTransitionInfo(transition), transitionData);
        //}
        public void ChangeState( StateTransitionTypeBinding<TState, TTransition,TOwner> transitionBinding,  object transitionData = null)
        {
            if (!currentState.Equals(transitionBinding.Info.From))
            {
                if (currentState.Equals(transitionBinding.Info.To))
                {
                    if (LastTransition.Transition.Info.Id.Equals(transitionBinding.Info.Id)
                        && LastTransition.TransitionData == transitionData)
                    {
                        // Okay, if transition is Last transition, and transitionData == Last transition transitionData, and this transition is idempotent (assumed true by default) 
                        return;
                    }
                }
                throw new StateMachineException($"Transition {transitionBinding.Info.Id} ({transitionBinding.Info.From} -> {transitionBinding.Info.To}) not valid from state {currentState}");
            }
            //var context = new MultiTypeSeed
            //var sc = new StateChange<TState, TTransition,TOwner>
            //{
            //    Transition = transitionBinding,
            //    TransitionData = transitionData,
            //    //StateChangeContext =
            //};

            transitionBinding.From?.OnLeaving(Owner);
            transitionBinding.OnTransitioningMethod(Owner);
            transitionBinding.To?.OnEntering(Owner);
            currentState = transitionBinding.Info.To;
            //transitionBinding.To?.OnEntered(Owner);
        }

    }

    //public struct MultiTypeSeed : IExtendableMultiTyped
    //{
    //private MultiTyped {get;set;}
    // private void Create() { MultiTyped = new MultiTyped(); }
    //}

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
