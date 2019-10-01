using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Structures
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SetFromChildAttribute : Attribute
    {
        public SetFromChildAttribute()
        {
        }

        public Type ChildType { get; set; }

    }
}
