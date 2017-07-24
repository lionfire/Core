using System.Collections.Generic;

namespace LionFire.StateMachines
{
    public interface IRemembersLastStateTransition
    {
        //StateChange LastStateChange { get; set; }
    }
    // Basis for undo/redo?
    public interface IRemembersStateTransitions<TState,TTransition> : IRemembersLastStateTransition
    {
        Queue<StateChange<TState, TTransition>> History { get; }
    }
}
