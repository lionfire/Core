using LionFire.Configuration;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;

namespace LionFire.Hosting;

public static class TelemetryHostingX
{
    public static ILionFireHostBuilder Tracing(this ILionFireHostBuilder lf)
    {
        //var appInfo = lf.AppInfo();

        var serviceName = lf.Configuration[AppConfigurationKeys.AppName] ?? throw new ArgumentNullException("Configuration: " + AppConfigurationKeys.AppName);
        var serviceVersion = lf.Configuration[AppConfigurationKeys.AppVersion] ?? throw new ArgumentNullException("Configuration: " + AppConfigurationKeys.AppVersion);
        
        lf.ConfigureServices(s => s
            .AddOpenTelemetry()
              .WithTracing(b =>
              {
                  b
                  .AddSource(serviceName)
                  .ConfigureResource(resource =>
                      resource.AddService(serviceName: serviceName, serviceVersion: serviceVersion))
                  //.AddAspNetCoreInstrumentation()
                  .AddConsoleExporter() // TEMP
                  ;
              })
        );

        return lf;
    }

    public static ILionFireHostBuilder Metrics(this ILionFireHostBuilder lf)
    {
        //var appInfo = lf.AppInfo();

        var serviceName = lf.Configuration[AppConfigurationKeys.AppName] ?? throw new ArgumentNullException("Configuration: " + AppConfigurationKeys.AppName);
        var serviceVersion = lf.Configuration[AppConfigurationKeys.AppVersion] ?? throw new ArgumentNullException("Configuration: " + AppConfigurationKeys.AppVersion);
        
        lf.ConfigureServices(s => s
            .AddOpenTelemetry()
            .WithMetrics(builder => builder
                //.AddAspNetCoreInstrumentation()
                .AddConsoleExporter()
                )
        );

        return lf;
    }
}

