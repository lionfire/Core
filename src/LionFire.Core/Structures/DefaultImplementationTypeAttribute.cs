using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Structures
{
    [System.AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    sealed class DefaultImplementationTypeAttribute : Attribute
    {
        public DefaultImplementationTypeAttribute(Type type)
        {
            this.type = type;
        }

        public Type Type
        {
            get { return type; }
        }
        private readonly Type type;
    }
}
