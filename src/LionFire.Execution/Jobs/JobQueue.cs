using LionFire.Collections.Concurrent;
using LionFire.Execution.Executables;
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
    public class JobQueue : ExecutableBase, IHasExecutionFlags, IExecutable, IControllableExecutable
    {
        #region State

        private ConcurrentList<IJob> unstartedJobs = new ConcurrentList<IJob>(); // REVIEW - make this a linkedlist if job count gets large?

        private ConcurrentHashSet<IJob> runningJobs = new ConcurrentHashSet<IJob>();
        private List<IJob> completedJobs = new List<IJob>();

        //public IEnumerable<IJob> ActiveJobs { get { return activeJobs.Keys; } }
        //private ConcurrentDictionary<IJob, IJob> activeJobs { get; set; } = new ConcurrentDictionary<IJob, IJob>();


        public bool TryAcceptJob(IJob job)
        {
            if (Prioritizer == null)
            {
                return true; // Accept all jobs if has no prioritizer
            }

            var priority = Prioritizer.GetPriority(job);
            if (double.IsNaN(priority))
            {
                return false; // Prioritizer can reject the job by returning NaN
            }
            Enqueue(job);
            return true;
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
            get;set;
        }

        public JobQueue()
        {
            MaxConcurrentJobs = System.Environment.ProcessorCount + 1;
        }

        public bool RequestStart(IJob job)
        {
            lock (lock_)
            {
                if (startingJob == job) return true;
                //if (ExecutionFlags.HasFlag(ExecutionFlag.AutoStart))
                //{
                //    TryStartJobs();
                //}
                return false;
            }
        }

        #region Methods

        public void Enqueue(IJob job)
        {
            if (job.Queue == this) return;
            if (job.Queue != null) throw new Exception("Job already in another queue");
            job.Queue = this;
            if (unstartedJobs.Contains(job)) return;
            JobManager.OnJobAdded(job);
            unstartedJobs.Add(job);
            OnJobEnqueued();
        }

        protected virtual void OnJobEnqueued()
        {
            if (ExecutionFlags.HasFlag(ExecutionFlag.AutoStart))
            {
                TryStartJobs();
            }
        }

        private object _lock = new object();  // TOTHREADSAFETY

        //public IJob EnqueueOrGet(IJob job)
        //{
        //    lock (_lock)
        //    {
        //        IJob result;

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

        object lock_ = new object();

        private IJob startingJob = null;
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
                    if (State != ExecutionState.Ready && State != ExecutionState.Started && State != ExecutionState.Starting)
                    {
                        throw new Exception($"Invalid state: {State}");
                    }

                    do
                    {
                        var highestPriorityJobs = GetHighestPriorityJobs();

                        foreach (var job in highestPriorityJobs.ToArray())
                        {
                            if (State != ExecutionState.Started) { State = ExecutionState.Starting; }
                            unstartedJobs.Remove(job);
                            startingJob = job;
                            job.Start();
                            jobsStarted++;
                            if (State != ExecutionState.Started) { State = ExecutionState.Started; }

                            if (job.RunTask != null && !job.RunTask.IsCompleted)
                            {
                                runningJobs.Add(job);
                                //while (!activeJobs.TryAdd(job, job))
                                //{
                                //    IJob replacement;
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

                    //IJob job;
                    //while (activeJobs.Count < MaxConcurrentJobs && unstartedJobs.TryDequeue(out job))
                    //{
                    //    job.Start();
                    //    jobsStarted++;
                    //    state.OnNext(ExecutionState.Started);
                    //    if (!job.RunTask.IsCompleted)
                    //    {
                    //        while (!activeJobs.TryAdd(job, job))
                    //        {
                    //            IJob replacement;
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

        public IEnumerable<IJob> GetHighestPriorityJobs(int maxJobs = 0)
        {
            if (maxJobs == 0) maxJobs = MaxConcurrentJobs - runningJobs.Count;

            var list = new SortedList<double, IJob>();

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

        protected void OnJobComplete(IJob job)
        {
            completedJobs.Add(job);
            runningJobs.Remove(job);
            JobManager.OnJobRemoved(job);
            if (ExecutionFlags.HasFlag(ExecutionFlag.AutoStart))
            {
                TryStartJobs();
            }
            if (runningJobs.Count == 0)
            {
                if (State == ExecutionState.Started)
                {
                    State = ExecutionState.Ready;
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
                foreach (var j in runningJobs.ToArray())
                {
                    await j.RunTask.ConfigureAwait(false);
                }
            }
        }

        #endregion

        public ExecutionFlag ExecutionFlags
        {
            get; set;
        } = ExecutionFlag.AutoStart;


        public ExecutionState DesiredExecutionState { get; set; }
        
    }
}
