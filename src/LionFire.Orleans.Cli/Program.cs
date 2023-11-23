using System;
using System.Net.Http;
using ConsoleAppFramework;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog.Configuration;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

var hostBuilder = Host.CreateDefaultBuilder();

ConsoleApp.CreateFromHostBuilder(hostBuilder, args).AddAllCommandType().Run();

