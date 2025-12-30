using LionFire.StateMachines.Class;
using LionFire.StateMachines;
using LionFire.Ontology;

namespace LionFire.Execution.Executables;

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
