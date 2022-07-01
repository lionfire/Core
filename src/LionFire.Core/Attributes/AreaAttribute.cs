using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public sealed class AreaAttribute : Attribute
    {
        public AreaAttribute(string area)
        {
            Area = area;
        }

        public string Area { get; }
    }
}
