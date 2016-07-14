using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public enum ExecutionHostState
    {
        Uninitialized, // includes Disposed
        Initialized,
        Initializing,
        Starting,
        Running,
        Pausing,
        Paused,
        Resuming,
        Stopping,
        Stopped,
        Aborting,
        Aborted,
        Finished,
    }


    public static class ExecutionHostStateExtensions
    {
        /// <summary>
        /// Returns true if it is desired for this service to be running (starting/started/resuming)
        /// </summary>
        public static bool IsActive(this ExecutionHostState state)
        {
            switch (state)
            {
                //case ServiceStatus.Uninitialized:
                //    break;
                //case ServiceStatus.Initialized:
                //    break;
                case ExecutionHostState.Starting:
                case ExecutionHostState.Running:
                case ExecutionHostState.Resuming:
                    return true;
                //case ServiceStatus.Pausing:
                //    break;
                //case ServiceStatus.Paused:
                //    break;
                //case ServiceStatus.Stopping:
                //    break;
                //case ServiceStatus.Stopped:
                //    break;
                //case ServiceStatus.Aborting:
                //    break;
                //case ServiceStatus.Aborted:
                //    break;
                default:
                    return false;
            }
        }
        public static bool IsRunning(this ExecutionHostState state)
        {
            switch (state)
            {
                //case ServiceStatus.Uninitialized:
                //    break;
                //case ServiceStatus.Initialized:
                //    break;
                case ExecutionHostState.Starting:
                case ExecutionHostState.Running:
                case ExecutionHostState.Pausing:
                case ExecutionHostState.Aborting:
                case ExecutionHostState.Resuming:
                case ExecutionHostState.Stopping:
                    return true;
                //case ServiceStatus.Paused:
                //    break;
                //case ServiceStatus.Stopped:
                //    break;
                //case ServiceStatus.Aborted:
                //    break;
                default:
                    return false;
            }
        }
    }
}
