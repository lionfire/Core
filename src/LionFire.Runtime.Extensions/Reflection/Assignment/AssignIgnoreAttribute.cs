using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AssignIgnoreAttribute : Attribute
    {
        public AssignIgnoreAttribute() { }
    }
}