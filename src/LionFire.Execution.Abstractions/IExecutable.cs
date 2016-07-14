using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    /// <summary>
    /// May also implement IPausable if such functionality is supported.
    /// </summary>
    public interface IExecutable
    {
        Task Start();
        Task Stop(StopMode mode = StopMode.GracefulShutdown, StopOptions options = StopOptions.StopChildren);
    }
}
