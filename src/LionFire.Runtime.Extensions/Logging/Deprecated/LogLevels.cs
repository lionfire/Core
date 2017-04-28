using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire
{
    public enum LogLevel
    {
        Disabled = 101,
        Fatal = 90,

        Critical = 80,

        Error = 70,

        Warning = 60,
        Warn = 60,

        MajorMessage = 53,
        Message = 50,
        MinorMessage = 47,

        Info = 40,

        Verbose = 30,

        Debug = 20,

        Trace = 10,
        Default = 9, // May not be a good idea
        Step = 5,
        All = 1,
        Unspecified = 0,

    }

    public static class LogLevelExtensions
    {
        public static LogLevel ToLogLevel(int level)
        {
            if (level > (int)LogLevel.Disabled) return LogLevel.Disabled;
            if (level > (int)LogLevel.Fatal) return LogLevel.Fatal;
            if (level > (int)LogLevel.Critical) return LogLevel.Critical;
            if (level > (int)LogLevel.Error) return LogLevel.Error;
            if (level > (int)LogLevel.Warn) return LogLevel.Warn;
            if (level > (int)LogLevel.MajorMessage) return LogLevel.MajorMessage;
            if (level > (int)LogLevel.Message) return LogLevel.Message;
            if (level > (int)LogLevel.MinorMessage) return LogLevel.MinorMessage;
            if (level > (int)LogLevel.Info) return LogLevel.Info;
            if (level > (int)LogLevel.Verbose) return LogLevel.Verbose;
            if (level > (int)LogLevel.Debug) return LogLevel.Debug;
            if (level > (int)LogLevel.Trace) return LogLevel.Trace;
            if (level > (int)LogLevel.Step) return LogLevel.Step;
            //if (level > (int)LogLevel.)
            return LogLevel.All;
        }
    }
}
