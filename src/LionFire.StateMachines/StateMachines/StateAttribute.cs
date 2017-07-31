using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.StateMachines
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class StateAttribute : Attribute
    {
        public bool StartingState { get; set; }

        public StateAttribute()
        {
        }
    }
}
