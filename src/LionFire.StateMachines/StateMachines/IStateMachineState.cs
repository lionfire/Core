using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.StateMachines
{
    public interface IStateMachineState<TState, TTransition>
    {
        TState CurrentState { get; }
        StateChange<TState,TTransition> LastTransition { get; }
    }

}
