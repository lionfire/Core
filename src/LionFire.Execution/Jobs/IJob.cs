using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public interface IJob : IStartable, IHasRunTask
    {
        bool IsCompleted { get; }
    }
    
}
