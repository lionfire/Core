using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.StateMachines
{
    [System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class StartAttribute : Attribute
    {
    }
    [System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EndAttribute : Attribute
    {
    }
}
