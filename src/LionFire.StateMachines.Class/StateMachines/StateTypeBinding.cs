using System;

namespace LionFire.StateMachines.Class
{
    public class StateTypeBinding<TState, TTransition, TOwner>
    {
        //public StateTransitionInfo<TState, TTransition> Info { get; set; }

        public StateTypeBinding(TState state)
        {
            //this.Info = StateMachine<TState, TTransition>.GetStateInfo(state);
        }
        public Action<TOwner> OnEntering { get; set; } = DefaultActions<TOwner>.DefaultAction;
        public Action<TOwner> OnLeaving { get; set; } = DefaultActions<TOwner>.DefaultAction;



    }
}