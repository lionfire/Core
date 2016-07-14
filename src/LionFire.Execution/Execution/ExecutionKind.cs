using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public enum ExecutionKind
    {
        Unspecified,

        Process,

        /// <summary>
        /// A script run in process, provided access to an environment for that script 
        /// </summary>
        Script,

        Plugin,
    }
}
