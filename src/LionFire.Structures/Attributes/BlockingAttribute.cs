using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire
{
    /// <summary>
    /// When defined on a method, indicates that it causes some sort of blocking behavior
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class BlockingAttribute : Attribute
    {        
        public string Alternative { get; set; }

        public BlockingAttribute() { }
        public BlockingAttribute(string alternative) { Alternative = alternative; }
    }
}
