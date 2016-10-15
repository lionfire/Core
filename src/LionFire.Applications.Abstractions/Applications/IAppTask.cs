using LionFire.Applications.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LionFire.Execution;

namespace LionFire.Applications
{
        


    public interface IAppTask : IAppComponent, IHasRunTask, IStartable
    {        

        /// <summary>
        /// App will continue running until the IAppTask.Task completes
        /// </summary>
        bool WaitForCompletion { get; }
        
        
    }
}
