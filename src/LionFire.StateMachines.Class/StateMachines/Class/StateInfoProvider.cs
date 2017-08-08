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
                // TODO
                //Info = StateMachine<TState, TTransition>.GetTransitionInfo(state),
                //OnTransitioningMethod = (owner) => owner.OnInitializing(),
                OnEntering = GetMethod(state, EnteringStatePrefixes),
                OnLeaving = GetMethod(state, LeavingStatePrefixes),
                //From = fromInfo,
                //To = toInfo,
            };
            return binding;
        }


        public static bool DefaultStrictMode = true;
        public MethodInfo GetMethod(TTransition state, IEnumerable<string> prefixes, bool? strictMode = null)
        {
            return _GetMethod(state.ToString(), prefixes, strictMode);
        }
        public MethodInfo GetMethod(TState state, IEnumerable<string> prefixes, bool? strictMode = null)
        {
            return _GetMethod(state.ToString(), prefixes, strictMode);
        }
        public MethodInfo _GetMethod(string stateOrTransitionName, IEnumerable<string> prefixes, bool? strictMode = null)
        {
            if (!strictMode.HasValue) strictMode = DefaultStrictMode;

            var fields = typeof(TOwner).GetMethods(BindingFlags.Public | BindingFlags.Static).ToDictionary(fi => fi.Name);

            foreach (var prefix in prefixes)
            {
                var fieldName = prefix + stateOrTransitionName;
                if (fields.ContainsKey(fieldName))
                {
                    return fields[fieldName];
                }
                if (strictMode.Value) break;
            }
            return null;
        }

        public List<string> CanLeaveStatePrefixes = new List<string> { "CanLeave" };
        public List<string> CanEnterStatePrefixes = new List<string> { "CanEnter" };
        public List<string> LeavingStatePrefixes = new List<string> { "OnLeaving", "Leaving", "After" };
        public List<string> EnteringStatePrefixes = new List<string> { "OnEntering", "Entering", "Before" };
        public List<string> CanTransitionPrefixes = new List<string> { "Can" };
        public List<string> OnTransitionPrefixes = new List<string> { "On", "During" };

        public bool IsStrictModeEnabled { get; set; }

        public virtual StateTransitionTypeBinding<TState, TTransition, TOwner> GetTransitionTypeBinding(TTransition transition)
        {
            var fi = typeof(TTransition).GetField(transition.ToString());
            var aTransition = fi.GetCustomAttribute<TransitionAttribute>();

            var fromInfo = GetStateTypeBinding((TState)aTransition.From);
            var toInfo = GetStateTypeBinding((TState)aTransition.To);

            var binding = new StateTransitionTypeBinding<TState, TTransition, TOwner>(transition)
            {
                Info = StateMachine<TState, TTransition>.GetTransitionInfo(transition),
                CanTransitionMethod = GetMethod(transition, CanTransitionPrefixes),
                OnTransitioningMethod = GetMethod(transition, OnTransitionPrefixes),
                From = fromInfo,
                To = toInfo,
            };
            return binding;
        }
    }
}
