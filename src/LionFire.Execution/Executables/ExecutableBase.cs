using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.ComponentModel;
using LionFire.Structures;
using LionFire.StateMachines.Class;
using LionFire.StateMachines;
using LionFire.MultiTyping;

namespace LionFire.Execution.Executables
{
    public class ExecutableBase : IExecutable2
    {
        #region State Machine

        public IStateMachine<ExecutionState2, ExecutionTransition> StateMachine => stateMachine;
        private IStateMachine<ExecutionState2, ExecutionTransition> stateMachine;

        IStateMachine<ExecutionState2, ExecutionTransition> IHas<IStateMachine<ExecutionState2, ExecutionTransition>>.Object => stateMachine;

        #endregion

        public ExecutableBase()
        {
            stateMachine = StateMachine<ExecutionState2, ExecutionTransition>.Create(this);
        }
    }
}
