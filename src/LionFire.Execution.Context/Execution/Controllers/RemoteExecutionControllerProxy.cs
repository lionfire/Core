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
        public IBehaviorObservable<ExecutionState> ExecutionState {
            get {
                throw new NotImplementedException();
            }
        }

        [SetOnce]
        public ExecutionContext ExecutionContext { get; set; }
        

        public Task<bool> Initialize()
        {
            throw new Exception("No remote transports available");
            //return false;
        }

        public async Task Start() { }
        public async Task Stop(StopMode mode = StopMode.GracefulShutdown, StopOptions options = StopOptions.StopChildren) { }

        #region ExecutionState

        public IBehaviorObservable<ExecutionState> ExecutionState {
            get {
                return executionState;
            }
        }
        private BehaviorObservable<ExecutionState> executionState = new BehaviorObservable<ExecutionState>();

        #endregion


    }
}
