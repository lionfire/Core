using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    [Flags]
    public enum ExecutionFlag
    {
        None = 0,
        WaitForRunCompletion = 1 << 0,
        WaitForShutdownCompletion = 1 << 1,
        AutoRestart = 1 << 2,
        /// <summary>
        /// Auto-start when configuration state receives some sort of action to perform
        /// </summary>
        AutoStart = 1 << 3,

        
    }

    public static class ExecutionFlagsExtensions
    {
        public static bool? WaitForRunCompletion(this object @object)
        {
            return @object.HasFlag(ExecutionFlag.WaitForRunCompletion);
        }
        public static bool? HasFlag(this object @object, ExecutionFlag flag)
        {
            var hef = @object as IHasExecutionFlags;
            if (hef == null) return null;
            return hef.ExecutionFlags.HasFlag(flag);
        }
    }
}
