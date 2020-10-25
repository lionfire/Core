using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.UI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ViewAttribute : Attribute
    {
        public ViewAttribute()
        {
        }

        public string DefaultViewName { get; set; }
    }
}
