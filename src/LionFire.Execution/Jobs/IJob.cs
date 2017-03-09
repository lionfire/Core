using LionFire.Execution.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public interface IJob : IStartable, IHasRunTask
    {
        JobQueue Queue { get; set; }
        bool IsCompleted { get; }
    }
    
}
