using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.DependencyInjection
{
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class TryInjectAttribute : Attribute
    {
        public TryInjectAttribute() { }
    }
}
