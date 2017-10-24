using LionFire.Execution.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    // REVIEW this -- does job really add anything?  Move Queue related members to IQueuableJob
    public interface IJob : IStartable, IHasRunTask
    {
        //JobQueue Queue { get; set; }

        ///// <summary>
        ///// Inform the job that its turn has arrived 
        ///// </summary>
        ///// <param name="queue"></param>
        ///// <returns>true if job is ready to be started, false if it should be deferred in the queue (if supported)</returns>
        //bool OnFrontOfQueue(JobQueue queue);

        bool IsCompleted { get; }
    }
    
}
