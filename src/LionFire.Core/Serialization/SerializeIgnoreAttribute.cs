using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire
{
    [Obsolete("Use Ignore(AllSerialization) instead")]
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class SerializeIgnoreAttribute : Attribute
    {
        public SerializeIgnoreAttribute() { }
    }
}
