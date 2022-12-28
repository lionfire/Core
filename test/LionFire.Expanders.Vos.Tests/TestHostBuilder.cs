using LionFire.Hosting;
using LionFire.Persistence.Filesystem;
using LionFire.Persisters.Expanders;
using LionFire.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using Serilog;
using Serilog.Events;
using System.Reflection;
using Serilog.Formatting.Compact;
using Serilog.Templates.Themes;
using Serilog.Templates;
using Serilog.Sinks.Loki;
using Serilog.Sinks.Loki.Labels;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog.Sinks.File.GZip;
using OpenTelemetry.Metrics;

#warning TODO: consolidate logging setup code

using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .AddMeter("LionFire.Vos")
    .AddConsoleExporter()
    .Build();

var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

Serilog.Log.Logger = new LoggerConfiguration()
          //.ReadFrom.Configuration(configuration)
          .MinimumLevel.Debug()
          //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
          //.Enrich.FromLogContext()
          .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}")
          .WriteTo.File(new CompactJsonFormatter(), $"z:/log/{Assembly.GetEntryAssembly()?.GetName().Name ?? Guid.NewGuid().ToString()}.log")
          .WriteTo.Debug()
          //.WriteTo.TestCorrelator()
          .CreateLogger();


public static class LionFireOpenTelemetryTracingX
{
    public static IServiceCollection AddOpenTelemetryTracingLF(this IServiceCollection services, string? serviceName = null, string? serviceVersion = null)
    {
        services
        .AddOpenTelemetry()
            .WithTracing(c =>

            c
                .AddConsoleExporter()
                .AddSource(serviceName ?? "MyCompany.MyProduct.MyService")
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: serviceName, serviceVersion: serviceVersion ?? "0.0.0"))
        //.AddHttpClientInstrumentation()
        //.AddAspNetCoreInstrumentation()
        //.AddSqlClientInstrumentation();
        );
        return services;
    }
}

public static class TestHostBuilder
{
    public static string DataDir
    {
        get
        {
            if (dataDir == null)
            {
                var loc = typeof(TestHostBuilder).Assembly.Location;
                var dir = Path.GetDirectoryName(loc) ?? throw new Exception("Couldn't get Assembly Location");
                dataDir = Path.GetFullPath("../../../../../test/data", dir);
                //dataDir = Path.Combine(dataDir, typeof(TestHostBuilder).Assembly.GetName().Name!);
            }
            return dataDir;
        }
    }
    private static string? dataDir;

    static string TestZipPath => Path.Combine(DataDir, "zip/TestSourceFile.zip");
    static string TestZipUrlPath => TestZipPath.Replace(":", "");

    public static readonly string ZipBaseVobReferenceString = $"vos:///{TestZipUrlPath}:";

    public static IHostBuilder H
    {
        get
        {

            if (Log.Logger.GetType().FullName == "Serilog.Core.Pipeline.SilentLogger")
            {
                var app = Assembly.GetExecutingAssembly()?.GetName().Name ?? Guid.NewGuid().ToString();

                Serilog.Log.Logger = new LoggerConfiguration()
                      //.ReadFrom.Configuration(configuration)
                      .MinimumLevel.Verbose()
                      .MinimumLevel.Override("LionFire.DependencyMachines", LogEventLevel.Warning)
                      .MinimumLevel.Override("Microsoft.Hosting", LogEventLevel.Warning)
                      .MinimumLevel.Override("Microsoft.Extensions.Hosting", LogEventLevel.Warning)
                      .Enrich.FromLogContext()
                      .WriteTo.Console(new ExpressionTemplate(
                          "[{@t:mm:ss} {@l:u2} {ToString(Substring(SourceContext, LastIndexOf(SourceContext, '.')+1), '        '):u10}] {@m}\n{@x}",
                          theme: TemplateTheme.Code))
                      //.WriteTo.File(new CompactJsonFormatter(), $"z:/log/{app}.log")
                      //.WriteTo.LokiHttp(() => new LokiSinkConfiguration()
                      //{
                      //    LokiUrl = "http://localhost:3100",
                      //    LogLabelProvider = new DefaultLogLabelProvider(),

                      //})
                      .WriteTo.File(formatter: new ExpressionTemplate(
                                  //"{ {@t, @mt, @l, @x, @a: '" + app + "', ..@p} }\n"),
                                  "{ {@t, @m, @l: if @l = 'Verbose' then 'trace' else if @l = 'Debug' then 'dbug' else if @l = 'Trace' then 'trace' else if @l = 'Information' then 'info' else if @l = 'Warning' then 'warn' else if @l = 'Error' then 'error' else if @l = 'Critical' then 'crit' else @l, @x, @sc: SourceContext, @a: '" + app + "', @p, SourceContext: undefined()} }\n"),
                      path: $"z:/log/{Assembly.GetExecutingAssembly()?.GetName().Name ?? Guid.NewGuid().ToString()}.log"
                      //, hooks: new GZipHooks()
                      )
                      //.WriteTo.File(new CompactJsonFormatter(), $"z:/log/test.log")
                      //.WriteTo.Console(new ExpressionTemplate(
                      //    "[{@t:mm:ss} {@l:u2} {SourceContext:u3}] {@m}\n{@x}",
                      //    theme: TemplateTheme.Code))

                      //.WriteTo.Console(outputTemplate: "[{Level:u2} {SourceContext}] {Message:lj}{NewLine}{Exception}")
                      //.WriteTo.Debug()
                      .CreateLogger();
                //Log.Logger.Debug("Logger is not configured. Either this is a unit test or you have to configure the logger");
                Log.Logger.Debug($"----------------------- UNIT TEST START @ {DateTime.Now.ToShortTimeString()} -----------------------");
            }

            return Host.CreateDefaultBuilder()
                .LionFire(lf => lf
                    .Vos()
                )
                .UseSerilog(Serilog.Log.Logger)
                //.UseSerilog()
                .ConfigureServices((c, s) => s
                    .Expansion()
                    .AddFilesystem()

                    .TryAddEnumerableSingleton<IExpanderPlugin, ZipPlugin>()

                    .AddExpanderMounter("/testdata/zip")
                    .VosMount("/testdata", DataDir.ToFileReference())

                    .AddSharpZipLib()
                    .AddNewtonsoftJson()
                )
                ;
        }
    }
}
