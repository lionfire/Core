using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.StateMachines.CodeGen
{
    public enum StateMachineOptions
    {
        None = 0,

        /// <summary>
        /// Generate missing methods such as Start, Run, Stop
        /// </summary>
        StateMethods = 1 << 0,
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class StateMachineAttribute : Attribute
    {
        public StateMachineOptions Generate { get; private set; }
        public StateMachineAttribute(StateMachineOptions generateFlags, object transitions)
        {
            this.Generate = generateFlags;
        }
    }
}
