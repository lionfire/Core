//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace LionFire.StateMachines
//{
//    public enum StateMachineOptions
//    {
//        None = 0,

//        /// <summary>
//        /// Generate missing methods such as Start, Run, Stop
//        /// </summary>
//        StateMethods = 1 << 0,

//        /// <summary>
//        /// Include all transitions from model, even if there are no handler methods and they are not mentioned in the transitionsAllowed parameter
//        /// </summary>
//        NoTransitionPrune = 1 << 1,
//    }


//    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
//    public sealed class StateMachineAttribute : Attribute
//    {
//        public StateMachineOptions Generate { get; private set; }
//        public object TransitionsAllowed { get; private set; }
//        public StateMachineAttribute(StateMachineOptions generateFlags, object transitionsAllowed = null)
//        {
//            this.Generate = generateFlags;
//            this.TransitionsAllowed = transitionsAllowed;
//        }
//    }
//}
