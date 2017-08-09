using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LionFire.StateMachines.Class
{

    public class StateInfoProvider<TState, TTransition, TOwner>
    {
        public static StateInfoProvider<TState, TTransition, TOwner> Default { get; set; } = new StateInfoProvider<TState, TTransition, TOwner>();

        public virtual StateTypeBinding<TState, TTransition, TOwner> GetStateTypeBinding(TState state)
        {
            var fi = typeof(TState).GetField(state.ToString());
            var aTransition = fi.GetCustomAttribute<StateAttribute>();

            var binding = new StateTypeBinding<TState, TTransition, TOwner>(state)
            {
                CanEnter = GetMethod(state, Conventions.CanEnterStatePrefixes),
                CanLeave = GetMethod(state, Conventions.CanLeaveStatePrefixes),
                OnEntering = GetMethod(state, Conventions.EnteringStatePrefixes),
                OnLeaving = GetMethod(state, Conventions.LeavingStatePrefixes),
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
                CanTransitionMethod = GetMethod(transition, Conventions.CanTransitionPrefixes),
                OnTransitioningMethod = GetMethod(transition, Conventions.OnTransitionPrefixes),
                From = fromInfo,
                To = toInfo,
            };
            return binding;
        }

        internal MethodInfo GetMethod(TTransition state, IEnumerable<string> prefixes)
        {
            return _GetMethod(state.ToString(), prefixes);
        }
        internal MethodInfo GetMethod(TState state, IEnumerable<string> prefixes)
        {
            return _GetMethod(state.ToString(), prefixes);
        }

        Dictionary<string, MethodInfo> methods;

        private MethodInfo _GetMethod(string stateOrTransitionName, IEnumerable<string> prefixes)
        {
            if (methods == null) methods = typeof(TOwner).GetMethods(BindingFlags.Public | BindingFlags.Instance).ToDictionary(fi => fi.Name);

            foreach (var prefix in prefixes)
            {
                var fieldName = prefix + stateOrTransitionName;
                if (methods.ContainsKey(fieldName))
                {
                    return methods[fieldName];
                }
            }
            return null;
        }

        public static StateMachineConventions Conventions
        {
            get => conventions ?? StateMachineConventions.DefaultConventions;
            set => conventions = value;
        }
        private static StateMachineConventions conventions;

   
    }
}
