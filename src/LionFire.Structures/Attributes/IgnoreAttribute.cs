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
        Network = 1 << 2,
        Copy = 1 << 3,
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
