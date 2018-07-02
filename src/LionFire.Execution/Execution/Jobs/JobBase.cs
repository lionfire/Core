using LionFire.DependencyInjection;
using LionFire.Execution.Executables;
using LionFire.Reactive;
using LionFire.Reactive.Subjects;
using LionFire.Validation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution.Jobs
{

    /// <summary>
    /// Registers with the JobManager (if present) and asks the JobManager if it can start.  If not, the JobManager will tell the Job later when it can start.
    /// </summary>
    public abstract class JobBase : ExecutableExBase, IJob, IExecutableEx, IQueueableJob, IHasStartBlockers
    {
        public abstract bool IsCompleted { get; }

        public JobQueue Queue { get; set; }


        public BlockerCollection StartBlockers { get; private set; }

        public Task RunTask { get; protected set; }
        public async Task Start()
        {
            // TODO: If initializable and hasn't initialized, do that
            if (this is IInitializable initializable)
            {
                if (!await initializable.Initialize().ConfigureAwait(false)) return;
            }
            else
            {
                SetState(State, ExecutionStateEx.Ready);
            }

            if (State == ExecutionStateEx.Starting) return;
            if (this.IsStarted()) return;
            SetState(ExecutionStateEx.Ready, ExecutionStateEx.Starting);

            StartBlockers = new BlockerCollection(); // REFACTOR - JobQueue invoke something to create this
            StartBlockers.Unblocked += StartBlockers_Unblocked;
            // / If can start right away, StartBlockers will be empty
            if (InjectionContext.Current.GetService<JobManager>()?.TryEnqueue(this) != true)
            {
                await StartContinuation();
            }

            //if (StartBlockers == null)
            //{
            //    //await StartBlockers?.Wait();
            //}
            //else
            //{
            //}

        }
        private async void StartBlockers_Unblocked()
        {
            await StartContinuation();
        }

        //private object startLock = new object();
        private async Task StartContinuation()
        {
            StartBlockers.Unblocked -= StartBlockers_Unblocked;
            StartBlockers = null;

            if (!await OnStarting().ConfigureAwait(false)) return;
            RunTask = Run();
            await OnStarted().ConfigureAwait(false);
            State = ExecutionStateEx.Started;
            if (RunTask != null)
            {
#pragma warning disable 4014
                RunTask.ContinueWith(_ => State = ExecutionStateEx.Stopped); // REVIEW
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

        ///// <summary>
        ///// If can start right away, StartingDelay will be null
        ///// </summary>
        ///// <returns></returns>
        //public bool RequestStart()
        //{
        //    var jm = Defaults.TryGet<JobManager>();
        //    if (jm == null) return true;

        //    if (!jm.RequestStart(this))
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //public bool OnFrontOfQueue(JobQueue queue)
        //{
        //    bool isStarting = State == ExecutionState.Starting;

        //    if (StartBlockers?.BlockingQueues != null)
        //    {
        //        //foreach (var blocker in StartingDelay.DelayBlockers.Issues.Where(b => b.DataContext == queue).ToArray())
        //        //{
        //        //    StartingDelay.DelayBlockers.Issues.Remove(blocker);
        //        //}
        //        StartBlockers.BlockingQueues.Remove(queue);
        //        if (StartBlockers.BlockingQueues.Count == 0)
        //        {
        //            var re = StartBlockers.ResetEvent;
        //            StartBlockers = null;
        //            re.Set();
        //        }

        //    }
        //    return true;
        //}

        public Exception ExecutionException { get; protected set; }

        private bool TryDoFinished()
        {
            if (State == ExecutionStateEx.Stopped)
            {
                return true;
            }
            if (State == ExecutionStateEx.Faulted)
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
            while (RunTask != null && !TryDoFinished())
            {
                await Task.Delay(delay).ConfigureAwait(false);
                delay = Math.Min(maxDelay, delay + 100);
            }
        }
    }
}
