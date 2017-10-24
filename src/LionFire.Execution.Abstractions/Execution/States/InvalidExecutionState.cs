﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{

    public class InvalidExecutionStateException : Exception
    {
        public ExecutionStateEx RequiredState { get; set; }
        public ExecutionStateEx CurrentState { get; set; }

        public InvalidExecutionStateException() { }
        public InvalidExecutionStateException(ExecutionStateEx requiredState, ExecutionStateEx currentState) { }
        public InvalidExecutionStateException(string message) : base(message) { }
        public InvalidExecutionStateException(string message, Exception inner) : base(message, inner) { }


        public override string ToString()
        {
            if (RequiredState != ExecutionStateEx.Unspecified || CurrentState != ExecutionStateEx.Unspecified)
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
