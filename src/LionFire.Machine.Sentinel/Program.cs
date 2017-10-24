using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using LionFire.Applications.Hosting;
using System.Collections.ObjectModel;
using LionFire.Supervising.Heartbeat;
using LionFire.Execution.Hosts;

namespace LionFire.Machine.Sentinel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new AppHost()
                 .Add(new AppInfo("LionFire", "Machine", "Sentinel"))
                 .AddJsonAssetProvider()
                .Add<AssetsHost<HeartbeatMonitor>>()
                .Add(BuildWebHost(args).Run)
                .Run();

            Console.WriteLine("AppHost finished");
            Console.ReadKey();
        }
                 //.AddInit(a=> a.ServiceProvider.AddSingleton<ISerializationProvider>(typeof(SerializationProvider)))

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
