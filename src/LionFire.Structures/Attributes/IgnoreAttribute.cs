using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire
{
    // RENAME: DataFlags
    [Flags]
    public enum LionSerializeContext : uint
    {
        None = 0,
        Persistence = 1 << 1,

        /// <summary>
        /// Network serialization to another software that understands the object.  If the other software doesn't understand it, see External.
        /// </summary>
        Network = 1 << 2,

        Copy = 1 << 3,

        /// <summary>
        /// For simple clients that do not understand the domain logic well enough to generate derived/read-only property values.  Such values will be serialized in this case.
        /// </summary>
        External = 1 << 4,

        // --
        AllSerialization = Persistence | Network,
        All = 0xffffffff,
    }

    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public sealed class IgnoreAttribute : Attribute
    {
        public readonly LionSerializeContext Ignore;

        public IgnoreAttribute(LionSerializeContext ignore = LionSerializeContext.All)
        {
            this.Ignore = ignore;
        }
    }
}
