using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution.Initialization
{
#if NET461
    [Serializable]
#endif
    public class ExecutionInitializationException : Exception
    {
        public ExecutionInitializationException() { }
        public ExecutionInitializationException(string message) : base(message) { }
        public ExecutionInitializationException(string message, Exception inner) : base(message, inner) { }

#if NET461
        protected ExecutionInitializationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif
    }
}
