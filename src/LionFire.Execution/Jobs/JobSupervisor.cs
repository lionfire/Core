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
        public static JobManager Instance { get { return Singleton<JobManager>.Instance; } }

        #region Construction

        public JobManager()
        {
            _readOnlyJobs = new ReadOnlyObservableCollection<Execution.IJob>(jobs);
        }

        #endregion

        #region Jobs

        public ReadOnlyObservableCollection<IJob> Jobs
        {
            get { return _readOnlyJobs; }
        }
        ReadOnlyObservableCollection<IJob> _readOnlyJobs;
        ObservableCollection<IJob> jobs = new ObservableCollection<IJob>();

        #endregion

        #region Task Registration


        private ConcurrentDictionary<Task, IJob> taskJob = new ConcurrentDictionary<Task, IJob>();
        public IJob GetJob(Task task)
        {
            IJob job;
            taskJob.TryGetValue(task, out job);
            return job;
        }

        public void RegisterJobTask(Task task, IJob job)
        {
            taskJob.TryAdd(task, job);
        }

        #endregion

    }
    public static class JobManagerExtensions
    {
        public static IJob GetJob(this Task task)
        {
            return JobManager.Instance.GetJob(task);
        }
        public static void RegisterJobTask(this Task task, IJob job)
        {
            JobManager.Instance.RegisterJobTask(task, job);
        }
    }
}
