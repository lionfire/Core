using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LionFire.StateMachines
{
    //public class StateTransition
    //{
    //    public string Key { get; private set; }
    //    public string CommonName { get; private set; }
    //    public string SpecificName { get; private set; }

    //    public StateTransition(string key)
    //    {
    //        this.Key = key;
    //        int indexOfDot = key.IndexOf('.');
    //        if (indexOfDot != -1)
    //        {
    //            var chunks = key.Split('.').Select(k => k.Trim());
    //            if (chunks.Count() != 2) throw new ArgumentException("key can only have one dot and there must be non-whitespace text on either side of it");
    //            CommonName = chunks.First();
    //            SpecificName = chunks.ElementAt(1);
    //        }
    //        else
    //        {
    //            CommonName = key;
    //        }
    //    }
    //}
    public class StateTransitionInfo<TState, TTransition>
    {
        public virtual string ActionName => Id.ToString();


        public TTransition Id { get; set; }
        public TState From { get; set; }
        public TState To { get; set; }
    }

    public class StateTransitionTypeBinding<TState, TTransition, TOwner>
    {
        public StateTransitionInfo<TState, TTransition> Info { get; set; }

        public StateTypeBinding<TState, TTransition, TOwner> From { get; set; }
        public StateTypeBinding<TState, TTransition, TOwner> To { get; set; }

        public StateTransitionTypeBinding(TTransition transition)
        {
            this.Info = StateMachine<TState, TTransition>.GetTransitionInfo(transition);

            //var mi = typeof(TTransition).GetMember(transition.ToString());
            //var attr = mi.GetCustomAttribute<TransitionAttribute>();
            //attr.From

        }
        public Action<TOwner> OnTransitioningMethod { get; set; } = DefaultActions<TOwner>.DefaultAction;

    }
    public class StateTypeBinding<TState, TTransition, TOwner>
    {
        //public StateTransitionInfo<TState, TTransition> Info { get; set; }

        public StateTypeBinding(TState state)
        {
            //this.Info = StateMachine<TState, TTransition>.GetStateInfo(state);
        }
        public Action<TOwner> OnEntering { get; set; } = DefaultActions<TOwner>.DefaultAction;
        public Action<TOwner> OnLeaving { get; set; } = DefaultActions<TOwner>.DefaultAction;



    }

    internal class DefaultActions<T>
    {
        public static Action<T> DefaultAction = o => { };
    }
}