using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LionFire.AccessPanel.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {

            new AppHost()
                .AddSerialization()
                .AddNewtonsoftJson()
                .AddFilesystemObjectBus()
                .RunNowAndWait(() =>
                {
                    
                    CreateWebHostBuilder(args).Build().Run();

                }).Wait();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
