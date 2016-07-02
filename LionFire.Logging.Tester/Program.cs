#define MicrosoftLoggers
#define Email

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Serilog;

namespace LionFire.Logging.Tester
{
    public class Program
    {
        static Microsoft.Extensions.Logging.ILogger logger;
        public static void Main(string[] args)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

#if MicrosoftLoggers

            loggerFactory.AddConsole()
                    .AddDebug();

#endif

            SerilogConfig.Configure(loggerFactory);


            logger = loggerFactory.CreateLogger<Program>();

            TestEachLevel();

            logger.LogInformation(" --- Logging tester finished.  Press a key to exit. --- ");
            Console.ReadKey();
        }



        public static void TestEachLevel()
        {
            logger.LogCritical("LogCritical");
            logger.LogError("LogError");
            logger.LogWarning("LogWarning");
            logger.LogInformation("LogInformation");
            logger.LogDebug("LogDebug");
            logger.LogTrace("LogTrace");
        }
    }
}
