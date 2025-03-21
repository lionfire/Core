using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Shell
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ShellPresenterAttribute : Attribute
    {
        public ShellPresenterAttribute()
        {
        }

        public string DefaultTabName { get; set; }
    }
}
