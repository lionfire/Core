﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public interface IControllableExecutable : IExecutable
    {
        // REVIEW - make this observable too?
        ExecutionState DesiredExecutionState { get; set; }
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

        public static async Task<StateChangeResult> SetDesiredStateAndWait(this IControllableExecutable e, ExecutionState state, CancellationToken? cancellationToken = null)
        {
            if (e.State.Value == state) return StateChangeResult.Success;

            //var onState = state =>
            //{
            //};
            //var disposable = e.State.Subscribe(onState);

            e.DesiredExecutionState = state;

            // For now, poll
            while (true)
            {
                var curState = e.State.Value;
                if (curState == state) return StateChangeResult.Success;
                if (curState == ExecutionState.Disposed || curState == ExecutionState.Faulted) return StateChangeResult.Failed;
                if (e.DesiredExecutionState != state) return StateChangeResult.Overridden;
                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested) return StateChangeResult.OperationCanceled;

               
                await Task.Delay(100).ConfigureAwait(false);
            }
        }
    }
}
