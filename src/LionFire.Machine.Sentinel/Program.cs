using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LionFire.Applications.Hosting;

namespace LionFire.Machine.Sentinel
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var app = new AppHost()
                .AddJsonAssetProvider()
            //.RunNowAndWait(async () => await Task.Run(()=>BuildWebHost(args).Run()))
            //;
            .RunNowAndWait(() => BuildWebHost(args).Run());
            //BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
