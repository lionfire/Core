﻿using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Exporter;

public class LionFireMemoryExporter<T> : InMemoryExporter<T>
    where T : class
{
    public LionFireMemoryExporter(ICollection<T> items) : base(items) { }
}

public class LionFireMetricReader : BaseExportingMetricReader
{
    public static int CreateCount = 0;
    public LionFireMetricReader(BaseExporter<Metric> exporter) : base(exporter)
    {
        CreateCount++;
    }

    public int CollectCount = 0;
    public bool DoCollect()
    {
        CollectCount++;
        return base.Collect();
    }
}

public static class LionFireOpenTelemetryTracingX // MOVE
{
    private static MeterProviderBuilder AddLionFireMemoryExporter(
            this MeterProviderBuilder builder, IServiceCollection services,
            ICollection<Metric>? exportedItems = null,
            MetricReaderOptions? metricReaderOptions = null)
    {
        exportedItems ??= new List<Metric>();

        var metricExporter = new LionFireMemoryExporter<Metric>(exportedItems);
        
        var metricReader = new LionFireMetricReader(metricExporter);
        services.AddSingleton(metricReader);
        services.AddSingleton(exportedItems);

        //var metricReader = PeriodicExportingMetricReaderHelper.CreatePeriodicExportingMetricReader(
        //    metricExporter,
        //    metricReaderOptions,
        //    DefaultExportIntervalMilliseconds,
        //    DefaultExportTimeoutMilliseconds);

        return builder.AddReader(metricReader);
    }

    public static IServiceCollection AddOpenTelemetryTracingLF(this IServiceCollection services, string? serviceName = null, string? serviceVersion = null, Action<TracerProviderBuilder>? configure = null)
    {
        services
        .AddOpenTelemetry()
            .WithMetrics(c => c
                //.AddConsoleExporter(o => o
                //.Targets = OpenTelemetry.Exporter.ConsoleExporterOutputTargets.Console
                //)
                .AddLionFireMemoryExporter(services)
                //.AddInMemoryExporter(ActivitiesExport.Metrics.Value ??= new(), c => c
                //    .PeriodicExportingMetricReaderOptions = new PeriodicExportingMetricReaderOptions
                //    {
                //        ExportIntervalMilliseconds = 1000,
                //        //ExportTimeoutMilliseconds
                //    })
                //.AddOtlpExporter(opts =>
                //{
                //    opts.Endpoint = new Uri(builder.Configuration["Otlp:Endpoint"]);
                //})

                .ConfigureResource(r => r
                    .AddService("LionFire.Test")
                    .AddService("LionFire.Vos")
                    .AddService("LionFire.Persistence.Filesystem")
                    .AddService("LionFire.Vos.RootManager")
                    .AddService("LionFire.Persisters.SharpZipLib.SharpZipLibExpander")


                )
                //.AddRuntimeInstrumentation()
                .AddMeter("LionFire.Test")
                .AddMeter("LionFire.Vos")
                .AddMeter("LionFire.Persistence.Filesystem")
                .AddMeter("LionFire.Persisters.SharpZipLib")
                .AddMeter("LionFire.Vos.RootManager")
                .AddMeter("LionFire.Persisters.SharpZipLib.SharpZipLibExpander")
                
            )
        //    .WithTracing(c => c
        //        .AddConsoleExporter()
        //        .AddInMemoryExporter(ActivitiesExport.Activities.Value ??= new())

        //        .AddSource(serviceName ?? "MyCompany.MyProduct.MyService")
        //        .ConfigureResource(r => r
        //                .AddService(serviceName: serviceName ?? "ExampleService", serviceVersion: serviceVersion ?? "0.0.0")
        //                .AddService("inmemory-test") // ?                
        //        )

        //         .ConfigureResource(r => r
        //             .AddService("inmemory-test")
        //             .AddService("LionFire.Test")                 
        //         )

        //        //.AddHttpClientInstrumentation()
        //        //.AddAspNetCoreInstrumentation()
        //        //.AddSqlClientInstrumentation();

        //        .If(configure != null, configure)
        //)
                .StartWithHost()
            ;
        return services;
    }
}
