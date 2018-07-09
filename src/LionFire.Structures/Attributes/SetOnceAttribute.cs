using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire
{
    /// <summary>
    /// When defined on a class, properties with the class as the type should not be set more than once
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class SetOnceAttribute : Attribute
    {
        
    }
}
