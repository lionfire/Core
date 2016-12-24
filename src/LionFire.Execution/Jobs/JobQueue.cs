using LionFire.Collections.Concurrent;
using LionFire.Reactive;
using LionFire.Reactive.Subjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution.Jobs
{

    /// <summary>
    /// By default, starts in ready state with Autorestart
    /// TODO: Start (goes to ready)/stop/pause
    /// </summary>
    /// <remarks>
    /// States:
    ///  - Ready
    ///  - Starting
    ///  - Started
    /// </remarks>
    public class JobQueue : IHasExecutionFlags, IExecutable, IControllableExecutable
    {
        #region State

        private ConcurrentQueue<IJob> jobQueue = new ConcurrentQueue<IJob>();

        public IEnumerable<IJob> ActiveJobs { get { return activeJobs.Keys; } }
        private ConcurrentDictionary<IJob, IJob> activeJobs { get; set; } = new ConcurrentDictionary<IJob, IJob>();
        private ConcurrentDictionary<IJob, IJob> completedJobs { get; set; } = new ConcurrentDictionary<IJob, IJob>();


        #endregion

        #region Settings

        public int MaxConcurrentJobs { get; set; } = 1;

        #endregion

        #region Methods

        /// <summary>
        /// Also consinder EnqueueOrGet to avoid adding duplicate jobs
        /// </summary>
        /// <param name="job"></param>
        public void Enqueue(IJob job)
        {
            jobQueue.Enqueue(job);
            OnJobEnqueued();
        }

        private void OnJobEnqueued()
        {
            if (ExecutionFlags.HasFlag(ExecutionFlag.AutoStart))
            {
                TryStartJobs();
            }
        }

        private object _lock = new object();  // TOTHREADSAFETY

        public IJob EnqueueOrGet(IJob job)
        {
            lock (_lock)
            {
                IJob result;

                if (activeJobs.TryGetValue(job, out result))
                {
                    return result;
                }
                if (completedJobs.TryGetValue(job, out result))
                {
                    return result;
                }
                // O(n) OPTIMIZE
                result = jobQueue.Where(j => j == job).FirstOrDefault();
                if (result != null) { return result; }

                Debug.WriteLine($"[JOB] job with hash: {job.GetHashCode()}: {job.ToString()}");

                jobQueue.Enqueue(job);
                OnJobEnqueued();
                return job;
            }
        }

        /// <summary>
        /// Must be invoked if ExecutionFlag.AutoStart is not set
        /// </summary>
        /// <returns>Number of jobs started</returns>
        public int TryStartJobs()
        {
            // TOTHREADSAFE

            int jobsStarted = 0;

            if (HasJobsWaiting)
            {
                if (State.Value != ExecutionState.Ready && State.Value != ExecutionState.Started && State.Value != ExecutionState.Starting && State.Value != ExecutionState.WaitingToStart)
                {
                    throw new Exception($"Invalid state: {State.Value}");
                }
                if (State.Value == ExecutionState.Ready)
                {
                    state.OnNext(ExecutionState.Starting);
                }

                IJob job;
                while (activeJobs.Count < MaxConcurrentJobs && jobQueue.TryDequeue(out job))
                {
                    job.Start();
                    jobsStarted++;
                    state.OnNext(ExecutionState.Started);
                    if (!job.RunTask.IsCompleted)
                    {
                        while (!activeJobs.TryAdd(job, job))
                        {
                            IJob replacement;
                            if (activeJobs.TryGetValue(job, out replacement)) { job = replacement; }
                        }
                        job.RunTask.ContinueWith(t =>
                        {
                            OnJobComplete(job);
                            state.OnNext(ExecutionState.Ready);
                        });
                    }
                }
            }
            return jobsStarted;
        }

        #endregion

        public bool HasJobsWaiting
        {
            get { return activeJobs.Count < MaxConcurrentJobs && jobQueue.Count > 0; }
        }

        /// <returns>True if Job was started, False if no Jobs are in queue</returns>



        protected void OnJobComplete(IJob job)
        {
            IJob _;
            completedJobs.TryAdd(job, job);
            activeJobs.TryRemove(job, out _);
            if (ExecutionFlags.HasFlag(ExecutionFlag.AutoStart))
            {
                TryStartJobs();
            }
            if (activeJobs.Count == 0)
            {
                if (State.Value == ExecutionState.Started)
                {
                    state.OnNext(ExecutionState.Ready);
                }
            }
        }
        //private void UpdateState()
        //{
        //    if (activeJobs.Count > 0)
        //    {
        //        //if
        //    }
        //}

        #region Methods

        public async Task WaitAll()
        {
            while (TryStartJobs() > 0)
            {
                foreach (var j in activeJobs.Keys.ToArray())
                {
                    await j.RunTask;
                }
            }
        }

        #endregion



        public ExecutionFlag ExecutionFlags
        {
            get; set;
        } = ExecutionFlag.AutoStart;

        

        public ExecutionState DesiredState { get; set; }
        public IBehaviorObservable<ExecutionState> State { get { return state; } }
        BehaviorObservable<ExecutionState> state = new BehaviorObservable<ExecutionState>(ExecutionState.Ready);
    }
}
