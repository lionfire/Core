using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Services
{
    public enum StopMode
    {
        Unspecified = 0,

        /// <summary>
        /// Normal shutdown
        /// </summary>
        GracefulShutdown = 1 << 0,

        /// <summary>
        /// Will be restarted immediately
        /// </summary>
        ImminentRestart = 1 << 1,

        /// <summary>
        /// A rapid shutdown is required (such as system reboot or impatient user)
        /// </summary>
        UrgentShutdown = 1 << 2,

        /// <summary>
        /// The process or system is unstable and as a precaution things are shutting down
        /// </summary>
        CriticalFailure = 1 << 3,

        /// <summary>
        /// Don't walk, run to the nearest exit
        /// </summary>
        Abort = 1 << 4,

        
    }

    public enum StopOptions
    {
        None = 0,

        StopChildren = 1 << 0,

    }
}
