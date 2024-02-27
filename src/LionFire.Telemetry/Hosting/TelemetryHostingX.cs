using LionFire.Configuration;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Logs;
using Microsoft.Extensions.Logging;

namespace LionFire.Hosting;

// TODO: Change these to all extend IHostApplicationBuilder and remove support for ILionFireHostBuilder

public static class TelemetryHostingX
{
    public static ILionFireHostBuilder Tracing(this ILionFireHostBuilder lf)
    {
        //var appInfo = lf.AppInfo();

        var serviceName = lf.Configuration[AppConfigurationKeys.AppName] ?? throw new ArgumentNullException("Configuration: " + AppConfigurationKeys.AppName);
        var serviceVersion = lf.Configuration[AppConfigurationKeys.AppVersion] ?? throw new ArgumentNullException("Configuration: " + AppConfigurationKeys.AppVersion);

        lf.IHostApplicationBuilder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        lf.ConfigureServices(s => s
            .AddOpenTelemetry()
              .WithTracing(tracing =>
              {
                  if (lf.IHostApplicationBuilder.Environment.IsDevelopment())
                  {
                      tracing.SetSampler(new AlwaysOnSampler());
                  }

                  tracing.AddAspNetCoreInstrumentation()
                       .AddGrpcClientInstrumentation()
                       .AddHttpClientInstrumentation();

                  tracing
                      .AddSource(serviceName)
                      .ConfigureResource(resource =>
                          resource.AddService(serviceName: serviceName, serviceVersion: serviceVersion))
                  ;
              })
        );

        return lf;
    }

    public static ILionFireHostBuilder OpenTelemetry(this ILionFireHostBuilder lf, Action<OpenTelemetryBuilder>? configure = null)
    {
        lf
            .Metrics()
            .Tracing()
            .IHostApplicationBuilder
                .AddOpenTelemetryExporters()
            ;

        if(configure != null)
        {
            lf.ConfigureServices(s => configure(s.AddOpenTelemetry()));
        }
        return lf;
    }
    public static ILionFireHostBuilder Metrics(this ILionFireHostBuilder lf)
    {
        //var appInfo = lf.AppInfo();

        var serviceName = lf.Configuration[AppConfigurationKeys.AppName] ?? throw new ArgumentNullException("Configuration: " + AppConfigurationKeys.AppName);
        var serviceVersion = lf.Configuration[AppConfigurationKeys.AppVersion] ?? throw new ArgumentNullException("Configuration: " + AppConfigurationKeys.AppVersion);
        
        lf.ConfigureServices(s => s
            .AddOpenTelemetry()
            .WithMetrics(metrics => metrics
                //.AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddBuiltInMeters()
                //.AddConsoleExporter()
                )
        );
        

        return lf;
    }

    private static MeterProviderBuilder AddBuiltInMeters(this MeterProviderBuilder meterProviderBuilder) =>
        meterProviderBuilder.AddMeter(
            "Microsoft.AspNetCore.Hosting",
            "Microsoft.AspNetCore.Server.Kestrel",
            "System.Net.Http");

    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        //builder.Services.AddConsoleExporter(); // TEMP

        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.Configure<OpenTelemetryLoggerOptions>(logging => logging.AddOtlpExporter());
            builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddOtlpExporter());
            builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddOtlpExporter());
        }

        builder.Services.AddOpenTelemetry()
           .WithMetrics(metrics => metrics.AddPrometheusExporter())
            .ConfigureResource(resource => resource
                .AddService(serviceName: builder.Environment.ApplicationName))
            .WithMetrics(//metrics => metrics
                //.AddAspNetCoreInstrumentation() // TODO: Add to WebHost
                //.AddConsoleExporter((exporterOptions, metricReaderOptions) => // TEMP
                //{
                //    metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
                //})
                )
            ;
        
        return builder;
    }
}

