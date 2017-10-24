using LionFire.MultiTyping;
using LionFire.StateMachines;
using LionFire.StateMachines.Class;

namespace LionFire.Execution.Executables
{
    public class ExecutableBase<TState, TTransition>
    {
        #region State Machine

        public IStateMachine<TState, TTransition> StateMachine
        {
            get
            {
                if (stateMachine == null)
                {
                    stateMachine = StateMachine<TState, TTransition>.Create(this);
                }
                return stateMachine;
            }
        }
        private IStateMachine<TState, TTransition> stateMachine;

        #endregion
    }
}
