using LionFire.Reactive;
using LionFire.Reactive.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
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

        public Task Start() { return Task.CompletedTask; }
        public Task Stop(StopMode mode = StopMode.GracefulShutdown, StopOptions options = StopOptions.StopChildren) { return Task.CompletedTask; }

        #region ExecutionState

        public IBehaviorObservable<ExecutionState> State {
            get {
                return executionState;
            }
        }
        private BehaviorObservable<ExecutionState> executionState = new BehaviorObservable<ExecutionState>();

        #endregion

    }
}
