// TODO: Once this gets fleshed out, move to Core? Or create shared library?

namespace LionFire.StateMachines
{
    public interface IStateMachine
{
    //bool CanTransition(IState from, IState to, object context);
    //Task Transition(IState from, IState to, object context = null);
}
    public interface IStateMachine<TState, TTransition>
    {
    }
}
