using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    // DEPRECTED
    public interface IStoppableEx
    {
        Task Stop(StopMode mode = StopMode.GracefulShutdown, StopOptions options = StopOptions.StopChildren);
    }
    public interface IStoppable
    {
        Task Stop();
    }
}
