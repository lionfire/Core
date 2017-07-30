using CodeGeneration.Roslyn;
using LionFire.StateMachines.Generation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.StateMachines
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    [CodeGenerationAttribute(typeof(StateMachineGenerator))]
    //[Conditional("CodeGeneration")]
    public sealed class StateMachineAttribute : Attribute
    {
        public StateMachineOptions Generate { get; private set; }
        public object TransitionsAllowed { get; private set; }
        public StateMachineAttribute(StateMachineOptions generateFlags, object transitionsAllowed = null)
        {
            this.Generate = generateFlags;
            this.TransitionsAllowed = transitionsAllowed;
        }
    }
}
