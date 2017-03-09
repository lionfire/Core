using LionFire.Reactive;
using LionFire.Reactive.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution.Jobs
{
    /// <summary>
    /// Registers with the JobManager (if present) and asks the JobManager if it can start.  If not, the JobManager will tell the Job later when it can start.
    /// </summary>
    public abstract class JobBase : IJob, IExecutable
    {
        public abstract bool IsCompleted { get; }

        public JobQueue Queue { get; set; }

        public Task RunTask { get; protected set; }
        public async Task Start()
        {
            // TODO: If initializable and hasn't initialized, do that
            var ii = this as IInitializable;
            if (ii != null)
            {
                if (!await ii.Initialize().ConfigureAwait(false)) return;
            }

            if (!RequestStart()) return;

            state.OnNext(ExecutionState.Starting);
            if (!await OnStarting().ConfigureAwait(false)) return;
            RunTask = Run();
            await OnStarted().ConfigureAwait(false);
            state.OnNext(ExecutionState.Started);
            if (RunTask != null)
            {
#pragma warning disable 4014
                RunTask.ContinueWith(_ => state.OnNext(ExecutionState.Finished)); // REVIEW
#pragma warning restore 0414
            }
        }


        /// <returns>Returns true if starting is allowed to proceed</returns>
        protected virtual Task<bool> OnStarting()
        {
            return Task.FromResult(true);
        }

        protected abstract Task Run();

        protected virtual Task OnStarted()
        {
            return Task.CompletedTask;
        }

        public bool RequestStart()
        {
            var jm = Defaults.TryGet<JobManager>();
            if (jm == null) return true;

            return jm.RequestStart(this);
        }

        public Exception ExecutionException { get; protected set; }

        private bool TryDoFinished()
        {
            if (state.Value == ExecutionState.Finished)
            {
                return true;
            }
            if (state.Value == ExecutionState.Faulted)
            {
                throw new Exception("Job faulted.  See inner exception for the exception thrown at the time it faulted.", ExecutionException);
            }
            return false;
        }

        public async Task Wait(CancellationToken? cancelationToken = null)
        {
            if (TryDoFinished()) return;

            int delay = 100;
            int maxDelay = 10000;
            while (RunTask == null && !TryDoFinished())
            {
                await Task.Delay(delay).ConfigureAwait(false);
                delay = Math.Min(maxDelay, delay + 100);
            }
        }


        public IBehaviorObservable<ExecutionState> State { get { return state; } }
        protected BehaviorObservable<ExecutionState> state = new BehaviorObservable<ExecutionState>(ExecutionState.Ready);

    }
}
