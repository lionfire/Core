using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace LionFire.Extensions.Logging.NLog
{
    public class NLogConfig
    {
        // TODO: Read config from config file and/or parameter

        public static void LoadDefaultConfig()
        {
            var config = new LoggingConfiguration();
#if NET
            {
                var targ = new NetworkTarget
                {
                    //Layout = "${message}",
                    Address = "tcp://localhost:4505",
                };

                config.AddTarget("tcp", targ);
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, targ);
            }
            {
                var targ = new NetworkTarget
                {
                    Address = "udp://localhost:9999",
                    //Layout = "${log4jxmlevent}"
                    //Layout = "${message}",
                };

                config.AddTarget("udp", targ);
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, targ);
            }
#endif
            {
                var targ = new FileTarget
                {
                    FileName = @"e:\temp\Trading.Agent.log",
                    Layout = "${message}"
                };

                config.AddTarget("file", targ);
                config.AddRule(LogLevel.Trace, LogLevel.Fatal, targ);
            }


            {
#if Console
                    // Step 2. Create targets and add them to the configuration 
                    var consoleTarget = new ColoredConsoleTarget();
                config.AddTarget("console", consoleTarget);
                consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
                    var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
                config.LoggingRules.Add(rule1);
#endif

                var fileTarget = new FileTarget();
                config.AddTarget("file", fileTarget);

                // Step 3. Set target properties 
                fileTarget.FileName = "${basedir}/file.txt";
                fileTarget.Layout = "${message}";

                // Step 4. Define rules


                var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
                config.LoggingRules.Add(rule2);
            }
            LogManager.Configuration = config;
        }

        public static void TestLevels()
        {
            // Example usage
            Logger logger = LogManager.GetLogger("Example");
            logger.Trace("trace log message");
            logger.Debug("debug log message");
            logger.Info("info log message");
            logger.Warn("warn log message");
            logger.Error("error log message");
            logger.Fatal("fatal log message");
        }
    }
}
