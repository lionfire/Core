using LionFire.Collections.Concurrent;
using LionFire.Execution.Executables;
using LionFire.Extensions.Logging;
using LionFire.Reactive;
using LionFire.Reactive.Subjects;
using LionFire.Validation;
using Microsoft.Extensions.Logging;
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
    public class JobQueue : ExecutableExBase, IHasExecutionFlags, IExecutableEx, IControllableExecutable, IInitializable2
    {
        #region State

        private ConcurrentList<IQueueableJob> unstartedJobs = new ConcurrentList<IQueueableJob>(); // REVIEW - make this a linkedlist if job count gets large?

        private ConcurrentHashSet<IQueueableJob> runningJobs = new ConcurrentHashSet<IQueueableJob>();
        private List<IQueueableJob> completedJobs = new List<IQueueableJob>();

        //public IEnumerable<IQueueableJob> ActiveJobs { get { return activeJobs.Keys; } }
        //private ConcurrentDictionary<IQueueableJob, IQueueableJob> activeJobs { get; set; } = new ConcurrentDictionary<IQueueableJob, IQueueableJob>();


        public (bool accepted, bool started) TryAcceptJob(IQueueableJob job)
        {
            if (Prioritizer != null)
            {
                var priority = Prioritizer.GetPriority(job);
                if (double.IsNaN(priority))
                {
                    return (false, false); // Prioritizer can reject the job by returning NaN
                }
            }
            return (true, Enqueue(job));
        }

        #endregion

        #region Settings

        /// <summary>
        /// Default: Logical CPU count + 1
        /// </summary>
        public int MaxConcurrentJobs { get; set; }

        #endregion

        public IJobPrioritizer Prioritizer { get; set; }

        public JobManager JobManager
        {
            get; set;
        }

        public JobQueue()
        {
            logger = this.GetLogger();
            MaxConcurrentJobs = System.Environment.ProcessorCount + 1;
        }

        public bool RequestStart(IQueueableJob job)
        {
            // OPTIMIZE - return true to start right away if the queue is empty and the job is going to be started now.

            //lock (lock_)
            //{
            //    //if (startingJob == job) return true;
            //    //if (ExecutionFlags.HasFlag(ExecutionFlag.AutoStart))
            //    //{
            //    //    TryStartJobs();
            //    //}
            //}
            return false;
        }

        #region Methods

        public bool Enqueue(IQueueableJob job)
        {
            if (job.Queue == this) return false;
            if (job.Queue != null) throw new Exception("Job already in another queue"); // ENH: allow multiple queues in the future

            job.Queue = this;

            if (unstartedJobs.Contains(job)) return false;

            if (job is IHasStartBlockers sb)
            {
                sb.StartBlockers.Add(this);
            }

            JobManager.OnJobAdded(job);
            unstartedJobs.Add(job);
            OnJobEnqueued(job);
            return false;
        }

        protected virtual void OnJobEnqueued(IQueueableJob job)
        {
            if (ExecutionFlags.HasFlag(ExecutionFlag.AutoStart))
            {
                TryStartJobs();
            }
        }

        private object _lock = new object();  // TOTHREADSAFETY

        //public IQueueableJob EnqueueOrGet(IQueueableJob job)
        //{
        //    lock (_lock)
        //    {
        //        IQueueableJob result;

        //        if (activeJobs.TryGetValue(job, out result))
        //        {
        //            return result;
        //        }
        //        if (completedJobs.TryGetValue(job, out result))
        //        {
        //            return result;
        //        }
        //        // O(n) OPTIMIZE
        //        result = jobQueue.Where(j => j == job).FirstOrDefault();
        //        if (result != null) { return result; }

        //        Debug.WriteLine($"[JOB] job with hash: {job.GetHashCode()}: {job.ToString()}");

        //        jobQueue.Enqueue(job);
        //        OnJobEnqueued();
        //        return job;
        //    }
        //}

        public Task<ValidationContext> Initialize()
        {
            if (State == ExecutionStateEx.Unspecified)
            {
                State = ExecutionStateEx.Ready;
            }
            return Task.FromResult<ValidationContext>(null);
        }

        object lock_ = new object();

        private List<IQueueableJob> startingJob = new List<IQueueableJob>();
        /// <summary>
        /// Must be invoked if ExecutionFlag.AutoStart is not set
        /// </summary>
        /// <returns>Number of jobs started</returns>
        public int TryStartJobs()
        {
            // TOTHREADSAFE
            lock (lock_)
            {
                int jobsStarted = 0;

                if (CanStartJob)
                {
                    if (State == ExecutionStateEx.Unspecified && ExecutionFlags.HasFlag(ExecutionFlag.AutoStart))
                    {
                        Initialize();
                    }
                    if (State != ExecutionStateEx.Ready && State != ExecutionStateEx.Started && State != ExecutionStateEx.Starting)
                    {
                        throw new Exception($"Invalid state: {State}");
                    }

                    do
                    {
                        var highestPriorityJobs = GetHighestPriorityJobs();

                        foreach (var job in highestPriorityJobs.ToArray())
                        {
                            if (!CanStartJob) break;
                            if (State != ExecutionStateEx.Started) { State = ExecutionStateEx.Starting; }
                            unstartedJobs.Remove(job);

                            if (job is IHasStartBlockers hsb)
                            {
                                hsb.StartBlockers.Remove(this);
                            }
                            else if (job is IStartable startable)
                            {
                                startable.Start();
                            }
                            else
                            {
                                throw new NotImplementedException("Don't know how to notify job that it should start: " + job.GetType().Name);
                            }
                            if (State != ExecutionStateEx.Started) { State = ExecutionStateEx.Started; }
                            jobsStarted++;
                            if (runningJobs.Count > MaxConcurrentJobs)
                            {
                                logger.LogError($"runningJobs.Count {runningJobs.Count} > MaxConcurrentJobs {MaxConcurrentJobs}");
                            }
                            //else
                            //{
                            //    logger.LogDebug($"runningJobs.Count {runningJobs.Count} <= MaxConcurrentJobs {MaxConcurrentJobs}");
                            //}

                            if (job.RunTask != null && !job.RunTask.IsCompleted)
                            {
                                runningJobs.Add(job);
                                //while (!activeJobs.TryAdd(job, job))
                                //{
                                //    IQueueableJob replacement;
                                //    if (activeJobs.TryGetValue(job, out replacement)) { job = replacement; }
                                //}
                                job.RunTask.ContinueWith(t =>
                                {
                                    OnJobComplete(job);
                                });
                            }
                        }
                    }
                    while (CanStartJob);

                    //IQueueableJob job;
                    //while (activeJobs.Count < MaxConcurrentJobs && unstartedJobs.TryDequeue(out job))
                    //{
                    //    job.Start();
                    //    jobsStarted++;
                    //    state.OnNext(ExecutionState.Started);
                    //    if (!job.RunTask.IsCompleted)
                    //    {
                    //        while (!activeJobs.TryAdd(job, job))
                    //        {
                    //            IQueueableJob replacement;
                    //            if (activeJobs.TryGetValue(job, out replacement)) { job = replacement; }
                    //        }
                    //        job.RunTask.ContinueWith(t =>
                    //        {
                    //            OnJobComplete(job);
                    //        });
                    //    }
                    //}
                }
                return jobsStarted;
            }
        }

        public IEnumerable<IQueueableJob> GetHighestPriorityJobs(int maxJobs = 0)
        {
            if (maxJobs == 0) maxJobs = MaxConcurrentJobs - runningJobs.Count;

            var list = new SortedList<double, IQueueableJob>();

            if (Prioritizer == null) { return unstartedJobs.Take(maxJobs); }

            double nextPriority = 1;
            foreach (var job in unstartedJobs)
            {
                var priority = Prioritizer.GetPriority(job);
                if (double.IsNaN(priority)) priority = nextPriority++;

                while (list.ContainsKey(priority))
                {
                    priority += 0.001;
                }

                list.Add(priority, job);
            }

            return list.Reverse().Select(kvp => kvp.Value);
        }

        #endregion

        public bool CanStartJob
        {
            get { return runningJobs.Count < MaxConcurrentJobs && unstartedJobs.Count > 0; } // REVIEW - move state check to here?
        }

        /// <returns>True if Job was started, False if no Jobs are in queue</returns>
        protected void OnJobComplete(IQueueableJob job)
        {
            try
            {
                completedJobs.Add(job);
                runningJobs.Remove(job);
                JobManager.OnJobRemoved(job);
            }
            finally
            {
                if (ExecutionFlags.HasFlag(ExecutionFlag.AutoStart))
                {
                    TryStartJobs();
                }
                if (runningJobs.Count == 0)
                {
                    SetState(ExecutionStateEx.Started, ExecutionStateEx.Ready);
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
            while (TryStartJobs() > 0|| unstartedJobs.Count > 0)
            {
                if (runningJobs.Count == 0)
                {
                    logger.LogWarning("WaitAll: Failed to start any job.  Delaying and trying again.");
                    await Task.Delay(5000);
                    continue;
                }

                foreach (var runningJob in runningJobs.ToArray())
                {
                    await runningJob.RunTask.ConfigureAwait(false);
                }
            }
        }

        #endregion

        public ExecutionFlag ExecutionFlags
        {
            get; set;
        } = ExecutionFlag.AutoStart;


        public ExecutionStateEx DesiredExecutionState { get; set; }

        protected ILogger logger;
    }
}
