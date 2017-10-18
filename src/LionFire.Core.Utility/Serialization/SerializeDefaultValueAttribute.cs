using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class SerializeDefaultValueAttribute : Attribute
    {
        readonly bool? serializeDefaultValue;

        public SerializeDefaultValueAttribute(bool serializeDefault = true)
        {
            this.serializeDefaultValue = serializeDefault;
        }

        public bool? SerializeDefaultValue
        {
            get { return serializeDefaultValue; }
        }
    }
}
