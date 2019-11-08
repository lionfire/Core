using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire
{
    /// <summary>
    /// When defined on a method, indicates that it should only be called by consumers of a method (via public access), and not by the containing or derived classes (via private or protected access).
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class PublicOnlyAttribute : Attribute
    {        
        public PublicOnlyAttribute() { }
    }
}
