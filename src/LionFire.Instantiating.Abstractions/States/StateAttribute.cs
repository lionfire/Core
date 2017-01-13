using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.States
{
    /// <summary>
    /// Put this on a class to indicate that there may be properties or fields with the State attribute.  Properties and Fields will be serialized as state for the object.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class StateAttribute : Attribute
    {
        public StateAttribute() { }
    }
}
