using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.CommandLine.Arguments
{
    [System.AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class CliOptionAttribute : CliArgumentAttribute
    {
        public CliOptionAttribute()
        {
            UsesOptionPrefix = true;
        }
    }
}
