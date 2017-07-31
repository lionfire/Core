using System.Reflection;

namespace LionFire.StateMachines.Tests
{
    public interface IStateInfoProvider
    {
    }
    public class StateInfoProvider<TState, TTransition, TOwner> : IStateInfoProvider
    {
        public static StateInfoProvider<TState, TTransition, TOwner> Default { get; set; } = new StateInfoProvider<TState, TTransition, TOwner>();

        public StateTypeBinding<TState, TTransition, TOwner> GetStateTypeBinding(TState state)
        {
            var fi = typeof(TTransition).GetField(transition.ToString());
            var aTransition = fi.GetCustomAttribute<StateAttribute>();
        }

        public StateTransitionTypeBinding<TState, TTransition, TOwner> GetTransitionTypeBinding(TTransition transition)
        {
            var fi = typeof(TTransition).GetField(transition.ToString());
            var aTransition = fi.GetCustomAttribute<TransitionAttribute>();

            var fromInfo = GetStateTypeBinding<TOwner>(aTransition.From);
            var toInfo = GetStateTypeBinding<TOwner>(aTransition.To);

            var binding = new StateTransitionTypeBinding<TState, TTransition, GeneratedExecutable>(transition)
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
