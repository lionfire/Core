using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.CommandLine.Arguments
{
    [System.AttributeUsage(AttributeTargets.Method
        //| AttributeTargets.Class FUTURE
        , Inherited = false, AllowMultiple = false)]
    public sealed class CliVerbAttribute : CliArgumentAttribute
    {
        public CliVerbAttribute()
        {
            IsExclusive = true;
        }
    }

}
