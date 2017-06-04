using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.DependencyInjection
{
    /// <summary>
    /// Indicate that an attempt should be made at the appropriate time to inject a value into a property or field, if possible.
    /// See also: DependencyAttribute for required dependencies
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class TryInjectAttribute : Attribute
    {
        public TryInjectAttribute() { }


    }

}
