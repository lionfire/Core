using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{

    public class InvalidExecutionStateException : Exception
    {
        public ExecutionState RequiredState { get; set; }
        public ExecutionState CurrentState { get; set; }

        public InvalidExecutionStateException() { }
        public InvalidExecutionStateException(ExecutionState requiredState, ExecutionState currentState) { }
        public InvalidExecutionStateException(string message) : base(message) { }
        public InvalidExecutionStateException(string message, Exception inner) : base(message, inner) { }


        public override string ToString()
        {
            if (RequiredState != ExecutionState.Unspecified || CurrentState != ExecutionState.Unspecified)
            {
                return $"Invalid execution state.  Required: {RequiredState} Current: {CurrentState}";
            }
            else
            {
                return "Invalid execution state.";
            }
        }
    }
}
