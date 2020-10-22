using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public static class IExecutableExtensions
    {
        /// <summary>
        /// Returns true if Initialize has been successfully called, and does not need to be called for the executable to run
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsInitialized(this IExecutableEx e)
        {
            switch (e.State)
            {
                //case ExecutionState.Unspecified:
                //    break;
                //case ExecutionState.Unconfigured:
                //    break;
                //case ExecutionState.InvalidConfiguration:
                //    break;
                //case ExecutionState.Uninitialized:
                //    break;
                //case ExecutionState.Initializing:
                //    break;
                case ExecutionStateEx.Ready:
                case ExecutionStateEx.Stopped:
                case ExecutionStateEx.Starting:
                case ExecutionStateEx.Started:
                case ExecutionStateEx.Pausing:
                case ExecutionStateEx.Paused:
                case ExecutionStateEx.Unpausing:
                case ExecutionStateEx.Stopping:
                    return true;
                //case ExecutionState.Disposed:
                //    break;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if a request to start has been received, and execution has not finished.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsStarted(this IExecutableEx e)
        {
            switch (e.State)
            {
                //case ExecutionState.Unspecified:
                //    break;
                //case ExecutionState.Unconfigured:
                //    break;
                //case ExecutionState.InvalidConfiguration:
                //    break;
                //case ExecutionState.Uninitialized:
                //    break;
                //case ExecutionState.Initializing:
                //    break;
                //case ExecutionState.Ready:
                //    break;
                //case ExecutionState.Stopped:
                //    break;
                //case ExecutionState.Finished:
                //    break;
                //case ExecutionState.Disposed:
                //    break;
                case ExecutionStateEx.Starting:
                case ExecutionStateEx.Started:
                case ExecutionStateEx.Pausing:
                case ExecutionStateEx.Paused:
                case ExecutionStateEx.Unpausing:
                case ExecutionStateEx.Stopping:
                    //case ExecutionState.WaitingToStart:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if finished successfully or unsuccessfully.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsFinished(this IExecutableEx e)
        {
            switch (e.State)
            {
                //case ExecutionState.Unspecified:
                //    break;
                //case ExecutionState.Unconfigured:
                //    break;
                //case ExecutionState.InvalidConfiguration:
                //    break;
                //case ExecutionState.Uninitialized:
                //    break;
                //case ExecutionState.Initializing:
                //    break;
                //case ExecutionState.Ready:
                //    break;
                case ExecutionStateEx.Stopped:
                    return true;
                //case ExecutionState.Disposed:
                //    break;
                //case ExecutionState.Starting:
                //case ExecutionState.Started:
                //case ExecutionState.Pausing:
                //case ExecutionState.Paused:
                //case ExecutionState.Unpausing:
                //case ExecutionState.Stopping:
                    //case ExecutionState.WaitingToStart:
                default:
                    return false;
            }
        }


        public static async Task Restart(this IExecutableEx e, Action actionDuringShutdown = null, StopMode stopMode = StopMode.ImminentRestart | StopMode.GracefulShutdown, StopOptions stopOptions = StopOptions.StopChildren, CancellationToken? cancellationToken = null)
        {
            var exFlags = e as IAcceptsExecutionStateFlags;

            if (exFlags != null) { exFlags.ExecutionStateFlags |= ExecutionStateFlags.Restarting; }

            if (e is IStoppableEx stoppableEx) { await stoppableEx.Stop(stopMode, stopOptions).ConfigureAwait(false); }
            else if (e is IStoppable stoppable)
            {
                await stoppable.StopAsync(cancellationToken).ConfigureAwait(false);
            }

            if (actionDuringShutdown != null) actionDuringShutdown();

            var startable = e as IStartable;
            if (startable != null) { await startable.StartAsync().ConfigureAwait(false); }

            if (exFlags != null) { exFlags.ExecutionStateFlags &= ~ExecutionStateFlags.Restarting; }

        }
    }
}
