using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Data
{
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class IdAttribute : Attribute
    {
        public IdAttribute()
        {
        }
    }
}
