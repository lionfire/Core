#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LionFire
{
    public interface ILogger
    {
        LogLevel LogLevel { get; set; }

        string Name { get; }

        #region Log methods

        //void Debug(string message, string category = null, int eventId = 0);

        //void Trace(string message, string category = null);

        //void Info(string message, string category = null);
        //void Info(Func<string> message, string category = null);

        //void Warn(string message);

        //void Error(string message, string category = null);

        //void Fatal(string message, string category = null);

        #endregion

        //void Debug(string message);

        //void Trace(string message);

        void Info(string message);
        void Info(Func<string> message);
        void Warn(string message);
        void Warn(Exception ex);
        void Error(string message);
        void Error(Exception ex);

        void Fatal(string message);

        //void Debug(string message, int eventId);
        //void Trace(string message, int eventId);
        //void Info(string message, int eventId);
        //void Info(Func<string> message, int eventId);
        //void Warn(string message, int eventId);
        //void Error(string message, int eventId);
        //void Fatal(string message, int eventId);

        void Log(LogLevel logLevel, string message);
    }
    public static class ILoggerExtensions
    {
        [Conditional("TRACE")]
        public static void Trace(this ILogger logger, Func<string> message)
        {
            logger.Trace(message());
        }

        [Conditional("TRACE")]
        public static void TraceWarn(this ILogger logger, string message)
        {
            logger.Warn(message);
        }

        [Conditional("TRACE")]
        public static void Trace(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Trace, message);
        }

        [Conditional("DEBUG")]
        public static void Debug(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Debug, message);
        }
    }
}

#endif