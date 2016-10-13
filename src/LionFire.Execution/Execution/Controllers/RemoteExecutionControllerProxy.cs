using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    

    public class RemoteExecutionControllerProxy : IExecutionController
    {
        public IObservable<ExecutionState> BExecutionState {
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

        public ExecutionState ExecutionState {
            get {
                return bExecutionState.Value;
            }
            set {
                bExecutionState.OnNext(value);
            }
        }

        public IObservable<ExecutionState> ExecutionStates {
            get {
                return bExecutionState;
            }
        }
        private BehaviorSubject<ExecutionState> bExecutionState = new BehaviorSubject<ExecutionState>(ExecutionState.Unspecified);

        #endregion


    }
}
