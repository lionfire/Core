using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.CommandLine.Arguments
{
    /// <summary>
    /// Short one line description of this item
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class CliDescriptionAttribute : CliArgumentAttribute
    {
        public CliDescriptionAttribute(string description)
        {
            this.Description = description;
        }

        
    }
}
