using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    [Flags]
    public enum ExecutionContextInitializationState
    {
        None = 0,

        /// <summary>
        /// Invoke ExecutionContext.Initialize() to inject the dependencies for this context so that it is ready to be started.
        /// </summary>
        Uninitialized = 1 << 7,

        /// <summary>
        /// All necessary aspects to execute the ExecutionContext have been completed and it is ready to be started (if it hasn't already been started, and if it hasn't been dispo)
        /// </summary>
        Initialized = 1 << 0,

        /// <summary>
        /// The code (which may be a remote service) could not be located or retrieved
        /// </summary>
        MissingCode = 1 << 1,

        /// <summary>
        /// The mechanism for executing the code could not be determined or is not available
        /// </summary>
        MissingExecutor = 1 << 2,

        /// <summary>
        /// The location on which the ExecutionContext is set to be run is not available
        /// </summary>
        MissingHost = 1 << 3,

        /// <summary>
        /// The current process lacks permissions to execute the ExecutionContext as configured
        /// </summary>
        InsufficientPermissions = 1 << 4,

        /// <summary>
        /// The ExecutionContext has either completed successfully or been torn down and is no longer ready for execution.  
        /// </summary>
        Disposed = 1 << 5,

        MissingConstructor = 1 << 6,

        MissingEverything = MissingConstructor | MissingCode | MissingExecutor | MissingHost,
    }
}
