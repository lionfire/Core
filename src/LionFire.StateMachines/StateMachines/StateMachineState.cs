using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.StateMachines
{

    public class StateMachineState<TState, TTransition, TOwner> : IStateMachineState<TState, TTransition, TOwner>
    {
        public TOwner Owner { get; private set; }
        public StateChange<TState, TTransition, TOwner> LastTransition { get; set; }


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
            private set
            {
                if (EqualityComparer<TState>.Default.Equals(value, currentState)) return;

                var oldState = currentState;
                currentState = value;
                StateChangedForFromTo?.Invoke(Owner, oldState, value);
            }
        }
        private TState currentState;

        public event Action<TOwner, TState, TState> StateChangedForFromTo;

        #endregion

        // TODO: Also offer TryChangeState 
        // FUTURE: goal seeking adapter

        //public void ChangeState(TTransition transition, object transitionData = null)
        //{
        //    ChangeState(stateMachine.GetTransitionInfo(transition), transitionData);
        //}
        public Task ChangeStateAsync(StateTransitionTypeBinding<TState, TTransition, TOwner> transitionBinding, object transitionData = null)
        {
            ChangeState_CheckAlready(transitionBinding, transitionData);

            throw new NotImplementedException();
            //return Task.CompletedTask;
        }

        private void ChangeState_CheckAlready(StateTransitionTypeBinding<TState, TTransition, TOwner> transitionBinding, object transitionData = null)
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
        }

        public void ChangeState(StateTransitionTypeBinding<TState, TTransition, TOwner> transitionBinding, object transitionData = null)
        {
            ChangeState_CheckAlready(transitionBinding, transitionData);

            //var context = new MultiTypeSeed

            var sc = new StateChange<TState, TTransition, TOwner>
            {
                Transition = transitionBinding,
                TransitionData = transitionData,
                //StateChangeContext =
            };

            transitionBinding.From?.OnLeaving?.Invoke(Owner);
            transitionBinding.OnTransitioningMethod?.Invoke(Owner);
            transitionBinding.To?.OnEntering?.Invoke(Owner);
            currentState = transitionBinding.Info.To;

            //transitionBinding.To?.OnEntered(Owner); // Subscribe to changed event for this.
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
