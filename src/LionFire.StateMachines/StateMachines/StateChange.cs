namespace LionFire.StateMachines
{
    //public class StateChange<TState, TTransition>
    //{
    //    public TTransition Transition { get; set; }
    //    public object StateChangeContext { get; set; }
    //}
    public class StateChange<TState, TTransition>
    {
        public StateTransitionInfo<TState, TTransition> Transition { get; set; }
        public object StateChangeContext { get; set; }
    }
}
