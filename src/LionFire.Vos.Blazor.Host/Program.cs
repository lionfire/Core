using LionFire.Hosting;
using LionFire.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.IO;

namespace LionFire.Vos.Blazor.Host
{
    public class Program
    {

        // REFACTOR - also in ValorPacksHost
        public static IConfigurationBuilder CreateConfigurationBuilder(string basePath = null)
            => new ConfigurationBuilder()
                 .SetBasePath(basePath ?? Path.GetDirectoryName(typeof(Program).Assembly.Location))
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        static IConfigurationRoot config;
        public static void Main(string[] args)
        {
            config = CreateConfigurationBuilder().Build();
            NLog.LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            VosHost.Create(args, defaultBuilder: true)
            //Host.CreateDefaultBuilder(args)
            //.AddPersisters()
                .UseDependencyContext()
                .ConfigureServices(services =>
                {
                    services
                    .AddPersisters()
                    .AddLogging(loggingBuilder =>
                    {
                        loggingBuilder
                            .ClearProviders()
                            .SetMinimumLevel(LogLevel.Trace)
                            .AddNLog(config);
                    });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<VosBlazorHostStartup>();
                });
    }
}
