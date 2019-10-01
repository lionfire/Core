using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Overlays
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
    public sealed class OverlayIgnoreAttribute : Attribute
    {
        public OverlayIgnoreAttribute()
        {
        }
    }

    //[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
    //public sealed class MergeIgnoreAttribute : Attribute
    //{
    //    public MergeIgnoreAttribute()
    //    {
    //    }
    //}
}
