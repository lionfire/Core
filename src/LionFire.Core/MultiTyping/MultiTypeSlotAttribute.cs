using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Types
{
    /// <summary>
    /// Use this attribute to treat a class as a base class or interface when adding to a MultiType object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class MultiTypeSlotAttribute : Attribute
    {
        readonly Type type;

        public MultiTypeSlotAttribute(Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// Type to treat this type as when adding to a MultiType object.
        /// </summary>
        public Type Type
        {
            get { return type; }
        }
    }
}
