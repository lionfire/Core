using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using LionFire.Serialization.Json.Newtonsoft;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LionFire.Vos.Api.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new AppHost()
                .Add<NewtonsoftJsonSerializer>()
                
                .Run(() => CreateWebHostBuilder(args).Build().Run());
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
