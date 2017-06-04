using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using MSLogger = Microsoft.Extensions.Logging.ILogger;

namespace LionFire
{
    public static class ILoggerBridgeExtensions
    {
        public static void Error(this MSLogger logger, string msg)
        {
            logger.LogError(msg);
        }
        public static void Error(this MSLogger logger, string msg, Exception ex)
        {
            logger.Log<object>(Microsoft.Extensions.Logging.LogLevel.Error, new EventId(), null, ex, null);
        }
        public static void Warn(this MSLogger logger, string msg)
        {
            logger.LogWarning(msg);
        }
        public static void Info(this MSLogger logger, string msg)
        {
            logger.LogInformation(msg);
        }

        public static void Debug(this MSLogger logger, string msg)
        {
            logger.LogDebug(msg);
        }

    }
}
