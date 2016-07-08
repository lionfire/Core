using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Services.Hosting
{
    public enum ServiceStatus
    {
        Uninitialized, // includes Disposed
        Initialized,
        Initializing,
        Starting,
        Started,
        Pausing,
        Paused,
        Resuming,
        Stopping,
        Stopped,
        Aborting,
        Aborted,
    }

}
