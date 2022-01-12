using Blazorise;
using LionFire.Hosting;
using LionFire.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.IO;

namespace LionFire.Vos.Blazor.Host;


public class Program
{
    //static IConfigurationRoot config;
    public static void Main(string[] args)
    {
        //NLog.LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));

        //NEW in .NET 6
        //var builder = WebApplication.CreateBuilder();
        ////var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args);

        //builder.Services
        //    .AddSingleton<string>("asdf")
        //    .AddSingleton<object>()
        //;
        //builder.Configuration
        //    .AddUserSecrets<Program>()
        //    ;
        ////builder.ConfigureWebHostDefaults(webBuilder=>
        ////{
        ////});
        //builder.Build().Run();
        

        VosHost.Create(args: args)
            .ConfigureHostConfiguration(config =>
                config
                    .SetBasePath(basePath ?? Path.GetDirectoryName(typeof(Program).Assembly.Location))
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build()
            )
            .LionFire(b => 
            {
                
            })
            .ConfigureAppConfiguration(appConfiguration =>
            {
                appConfiguration
                    .SetBasePath(basePath ?? Path.GetDirectoryName(typeof(Program).Assembly.Location))
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    ;
            })
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
