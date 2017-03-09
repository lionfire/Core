
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution.Jobs
{
    public interface IJobPrioritizer
    {
        /// <summary>
        /// Get priority for Job (higher priorities will be executed first.)  Returns NaN if job is not relevant to this prioritizer.
        /// </summary>
        double GetPriority(IJob job);
    }
}
