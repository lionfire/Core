using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using LionFire.Application;

namespace LionFire.Conductor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ILoggerFactory loggerFactory = new LoggerFactory()
            .AddConsole()
            .AddDebug();

            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Conductor host starting.");

            var app = new LionApp();
            app.Run();

            
        }
    }
}
