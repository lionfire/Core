using LionFire.Structures;
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

        public static JobManager Instance { get { return Singleton<JobManager>.Instance; } }

        #endregion

        public List<JobQueue> ListeningQueues => listeningQueues;
        private List<JobQueue> listeningQueues = new List<JobQueue>();

        #region Construction

        public JobManager()
        {
            _readOnlyJobs = new ReadOnlyObservableCollection<Execution.IJob>(jobs);
        }

        #endregion

        internal void OnJobAdded(IJob job)
        {
            jobs.Add(job);
        }

        internal void OnJobRemoved(IJob job)
        {
            jobs.Remove(job);
        }
        

        #region Jobs

        public ReadOnlyObservableCollection<IJob> Jobs
        {
            get { return _readOnlyJobs; }
        }

        public bool RequestStart(IJob job)
        {
            if (job.Queue != null) { return job.Queue.RequestStart(job); }
            else
            {
                foreach (var queue in listeningQueues)
                {
                    if (queue.TryAcceptJob(job))
                    {
                        return queue.RequestStart(job);
                    }
                }
            }

            // No Queue willing to take the job, so let the job start on its own
            return true;
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




    }
    public static class JobManagerExtensions
    {
        public static IJob GetJob(this Task task)
        {
            return JobManager.Instance.GetJob(task);
        }
        //public static void RegisterJobTask(this Task task, IJob job)
        //{
        //    JobManager.Instance.RegisterJobTask(task, job);
        //}
    }
}
