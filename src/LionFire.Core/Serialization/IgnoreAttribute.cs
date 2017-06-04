using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire
{
    [Flags]
    public enum LionSerializeContext : uint
    {
        None = 0,
        Persistence = 1 << 1,
        Network = 1 << 2,
        Copy = 1 << 3,
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
