using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Types
{
    [Flags]
    public enum InterfaceKind
    {
        None,
        /// <summary>
        /// The primary interface type for an object.  Typically there is only one, and validation rules may enforce this.
        /// </summary>
        Primary = 1 << 0,

        /// <summary>
        /// Mixins accomplish the spirit of multiple inheritance
        /// </summary>
        Mixin = 1 << 1,

        /// <summary>
        /// Components are child items
        /// </summary>
        Component = 1 << 2,
    }
}
