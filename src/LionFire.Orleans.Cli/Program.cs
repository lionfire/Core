using ConsoleAppFramework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Configuration;
using System;
using System.Net.Http;
using System.Runtime.InteropServices;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

var hostBuilder = Host.CreateDefaultBuilder();
//hostBuilder.ConfigureServices(s=>s.AddLogging())

using var host = hostBuilder.Build(); 
using var scope = host.Services.CreateScope();
ConsoleApp.ServiceProvider = scope.ServiceProvider;

var consoleApp = ConsoleApp.Create();
consoleApp.Run(args);
//ConsoleApp.CreateFromHostBuilder(hostBuilder, args).AddAllCommandType().Run();

