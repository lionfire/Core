using LionFire.Dependencies;
using LionFire.Extensions.Logging;
using LionFire.Structures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution.Jobs
{
    public class JobManager
    {
        #region Static

        public static JobManager Default => DependencyLocator.Get<JobManager>();

        #endregion

        public List<JobQueue> ListeningQueues => listeningQueues;
        private List<JobQueue> listeningQueues = new List<JobQueue>();

        #region Construction


        public JobManager()
        {
            _readOnlyJobs = new ReadOnlyObservableCollection<Execution.IJob>(jobs);
            logger = this.GetLogger();
        }

        #endregion

        internal void OnJobAdded(IJob job)
        {
            jobs.Add(job);
            //logger.LogInformation($"[job++] {jobs.Count} {_readOnlyJobs.Count}");
        }

        internal void OnJobRemoved(IJob job)
        {
            jobs.Remove(job);
            //logger.LogDebug($"[job--] {jobs.Count} {_readOnlyJobs.Count}");
        }

        public int JobCount => jobs.Count;

        #region Jobs

        public ReadOnlyObservableCollection<IJob> Jobs
        {
            get { return _readOnlyJobs; }
        }

        ///// <summary>
        ///// Return true if job has started
        ///// </summary>
        ///// <param name="job"></param>
        ///// <returns></returns>
        //public bool RequestStartQueue(IQueueableJob job)
        //{
        //    if (job.Queue != null) { return true; }
        //    else
        //    {
        //        foreach (var queue in listeningQueues)
        //        {
        //            (var accepted, var started) = queue.TryAcceptJob(job);
        //            if (
        //                )
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    // No Queue willing to take the job, so let the job start on its own
        //    return true;
        //}


        /// <summary>
        /// Return true if the job can start now, false if it may need to wait on StartBlockers
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public bool TryEnqueue(IQueueableJob job)
        {
            if (job.Queue != null) { job.Queue.RequestStart(job); return true; }
            else
            {
                foreach (var queue in listeningQueues)
                {
                    var result = queue.TryAcceptJob(job);
                    if (result.accepted)
                    {
                        return true;
                    }
                }
            }

            // No Queue willing to take the job, so let the job start on its own
            return false;
        }

        ReadOnlyObservableCollection<IJob> _readOnlyJobs;
        ObservableCollection<IJob> jobs = new ObservableCollection<IJob>();

        #endregion

        #region Task Registration


        private ConcurrentDictionary<Task, IJob> taskJob = new ConcurrentDictionary<Task, IJob>();
        private ConcurrentDictionary<IJob, Task> jobTasks = new ConcurrentDictionary<IJob, Task>();

        public IJob GetJob(Task task)
        {
            IJob job;
            taskJob.TryGetValue(task, out job);
            return job;
        }

        //public void RegisterJobTask(Task task, IJob job)
        //{
        //    var task = job.RunTask;
        //    if (task != null)
        //    {
        //        jobTasks.AddOrUpdate(job, task, (x, y) => task);
        //        taskJob.TryAdd(task, job);
        //    }
        //}

        public void AddQueue(JobQueue queue)
        {
            listeningQueues.Add(queue);
            queue.JobManager = this;
        }

        public void AddQueueWithPrioritizer<TPrioritizer>()
            where TPrioritizer : class, IJobPrioritizer, new()
        {
            var queue = new JobQueue()
            {
                Prioritizer = new TPrioritizer()
            };
            listeningQueues.Add(queue);
        }

        #endregion

        protected ILogger logger;
    }
    public static class JobManagerExtensions
    {
        public static IJob GetJob(this Task task)
        {
            return JobManager.Default.GetJob(task);
        }
        //public static void RegisterJobTask(this Task task, IJob job)
        //{
        //    JobManager.Instance.RegisterJobTask(task, job);
        //}
    }
}
