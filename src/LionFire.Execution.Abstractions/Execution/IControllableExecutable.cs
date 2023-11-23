using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public interface IControllableExecutable : IExecutableEx
    {
        // REVIEW - make this observable too?
        ExecutionStateEx DesiredExecutionState { get; set; }
    }

    public enum StateChangeResult
    {
        Unspecified,

        Success,

        /// <summary>
        /// See fault info on object for more details
        /// </summary>
        Failed,

        /// <summary>
        /// Another client changed the DesiredExecutionState
        /// </summary>
        Overridden,

        /// <summary>
        /// CancelationToken canceled.
        /// </summary>
        OperationCanceled,
    }


    public static class IControllableExecutableExtensions
    {

        public static async Task<StateChangeResult> SetDesiredStateAndWait(this IControllableExecutable e, ExecutionStateEx state, CancellationToken cancellationToken = default)
        {
            if (e.State == state) return StateChangeResult.Success;

            //var onState = state =>
            //{
            //};
            //var disposable = e.State.Subscribe(onState);

            e.DesiredExecutionState = state;

            // For now, poll
            while (true)
            {
                var curState = e.State;
                if (curState == state) return StateChangeResult.Success;
                if (curState == ExecutionStateEx.Disposed || curState == ExecutionStateEx.Faulted) return StateChangeResult.Failed;
                if (e.DesiredExecutionState != state) return StateChangeResult.Overridden;
                if (cancellationToken != default && cancellationToken.IsCancellationRequested) return StateChangeResult.OperationCanceled;

               
                await Task.Delay(100).ConfigureAwait(false);
            }
        }
    }
}
