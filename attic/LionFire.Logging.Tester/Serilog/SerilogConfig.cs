
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Logging.Tester
{
    public class SerilogConfig
    {


        [Conditional("Serilog")]
        public static void Configure(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog(); // Uses static serilog logger

            var serilogDir = LionEnvironment.Directories.LogsDir;

            Log.Logger = new LoggerConfiguration()
                //.WriteTo.ColoredConsole()
                .WriteTo.File(Path.Combine(serilogDir, LionEnvironment.ProgramName + ".log"))
                .CreateLogger();
            

        }
    }
}
