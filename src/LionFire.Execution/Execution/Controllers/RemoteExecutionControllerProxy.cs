using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    

    public class RemoteExecutionControllerProxy : IExecutionController
    {

        [SetOnce]
        public ExecutionContext ExecutionContext { get; set; }

        public Task<bool> Initialize()
        {
            throw new Exception("No remote transports available");
            //return false;
        }

        public async Task Start() { }
        public async Task Stop(StopMode mode = StopMode.GracefulShutdown, StopOptions options = StopOptions.StopChildren) { }

    }
}
