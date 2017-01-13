using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    // FUTURE: State machine with states at nodes and states at edges?

    public enum ExecutionState
    {
        Unspecified = 0,

        //Unconfigured = 1 << 0,

        Uninitialized = 1 << 2,
        /// <summary>
        /// Next: fix configuration/state, and initialize and start
        /// </summary>
        Faulted = 1 << 16,

        Initializing = 1 << 3,
        Ready = 1 << 4,

        Starting = 1 << 5,
        Started = 1 << 6,

        Pausing = 1 << 7,
        Paused = 1 << 8,

        Unpausing = 1 << 9,

        Stopping = 1 << 10,
        Stopped = 1 << 11,

        /// <summary>
        /// Completed.  Needs to be initialized
        /// </summary>
        Finished = 1 << 15,

        Disposed = 1 << 12,

        
    }

    public interface IHasExecutionState
    {
        ExecutionState ExecutionState { get; }
    }
    public interface IAcceptsExecutionState
    {
        ExecutionState ExecutionState { set; }
    }
    
    public interface IChangesExecutionState
    {
        IObservable<ExecutionState> ExecutionStates { get; }
    }

    //public interface ICancelableExecutionState
    //{
    //    CancellationToken ExecutionStateCancellationToken
    //RegisterCancellationToken(ExecutionState state, CancellationToken ExecutionStateCancellationToken);
    //}
}
