using System.Reflection;

namespace LionFire.StateMachines.ClassState
{
    //public interface IStateInfoProvider
    //{
    //}
    public class StateInfoProvider<TState, TTransition, TOwner> //: IStateInfoProvider
    {
        public static StateInfoProvider<TState, TTransition, TOwner> Default { get; set; } = new StateInfoProvider<TState, TTransition, TOwner>();

        public virtual StateTypeBinding<TState, TTransition, TOwner> GetStateTypeBinding(TState state)
        {
            var fi = typeof(TState).GetField(state.ToString());
            var aTransition = fi.GetCustomAttribute<StateAttribute>();

            var binding = new StateTypeBinding<TState, TTransition, TOwner>(state)
            {
                //Info = StateMachine<TState, TTransition>.GetTransitionInfo(state),
                //OnTransitioningMethod = (owner) => owner.OnInitializing(),
                //From = fromInfo,
                //To = toInfo,
            };
            return binding;
        }

        public virtual StateTransitionTypeBinding<TState, TTransition, TOwner> GetTransitionTypeBinding(TTransition transition)
        {
            var fi = typeof(TTransition).GetField(transition.ToString());
            var aTransition = fi.GetCustomAttribute<TransitionAttribute>();

            var fromInfo = GetStateTypeBinding((TState)aTransition.From);
            var toInfo = GetStateTypeBinding((TState)aTransition.To);

            var binding = new StateTransitionTypeBinding<TState, TTransition, TOwner>(transition)
            {
                Info = StateMachine<TState, TTransition>.GetTransitionInfo(transition),
                OnTransitioningMethod = (owner) => owner.OnInitializing(),
                From = fromInfo,
                To = toInfo,
            };
            return binding;
        }
    }
}
