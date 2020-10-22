using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Logging
{
    public interface IAppStartLogger
    {

        /// <summary>
        /// Applies to start log message
        /// </summary>
        bool? LogSucceeded { get; }

        /// <summary>
        /// Exception or error if start log message fails
        /// </summary>
        object LogError { get; }
    }

}
