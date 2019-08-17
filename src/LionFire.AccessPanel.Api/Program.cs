using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using LionFire.Hosting;
using LionFire.ObjectBus;
using LionFire.ObjectBus.Filesystem;
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

            FrameworkHostBuilder.Create()
                .AddObjectBus<FSOBus>()
                .Run(() =>
                {                    
                    CreateWebHostBuilder(args).Build().Run();
                }).Wait();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
