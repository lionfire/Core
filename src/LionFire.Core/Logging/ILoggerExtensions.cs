using System;
using System.Diagnostics;

namespace Microsoft.Extensions.Logging
{
    public static class ILoggerExtensions
    {
        [Conditional("TRACE")]
        public static void TraceWarn(this ILogger logger, string msg)
        {
            logger.LogWarning(msg);
        }
    }
}
