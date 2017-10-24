namespace LionFire.Execution.Jobs
{
    public interface IQueueableJob : IJob
    {
        JobQueue Queue { get; set; }

        ///// <summary>
        ///// Inform the job that its turn has arrived 
        ///// </summary>
        ///// <param name="queue"></param>
        ///// <returns>true if job is ready to be started, false if it should be deferred in the queue (if supported)</returns>
        //bool OnFrontOfQueue(JobQueue queue);
    }
}
