using Blazorise;
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
        public static IConfiguration CreateConfigurationBuilder(string basePath = null)
            => new ConfigurationBuilder()
                 .SetBasePath(basePath ?? Path.GetDirectoryName(typeof(Program).Assembly.Location))
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

        //static IConfigurationRoot config;
        public static void Main(string[] args)
        {
            //config = CreateConfigurationBuilder().Build();
            //NLog.LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
                
            VosHost.Create(config: CreateConfigurationBuilder(), args: args)
                .UseDependencyContext()
                .ConfigureServices(services =>
                {
                    services
                    .AddPersisters()
                    //.AddLogging(loggingBuilder =>
                    //{
                    //    loggingBuilder
                    //        .ClearProviders()
                    //        .SetMinimumLevel(LogLevel.Trace)
                    //        .AddNLog(config);
                    //.AddNLogWeb()
                    //})
                    //.AddBlazorise(options =>
                    //{
                    //    options.ChangeTextOnKeyPress = true;
                    //})
                    ;
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<VosBlazorHostStartup>();
                })
                .Build()
                .Run();
        }
    }
}
