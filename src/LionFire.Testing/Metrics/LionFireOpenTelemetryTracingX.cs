using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

public static class LionFireOpenTelemetryTracingX
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
                .AddOtlpExporter(option =>
                {
                    option.Endpoint = new Uri("http://localhost:4317");
                //    opts.Endpoint = new Uri(builder.Configuration["Otlp:Endpoint"]);
                    option.ExportProcessorType = ExportProcessorType.Batch;
                    option.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                    //option.Headers = $"x-example-apikey={apiKey}";
                })

                .ConfigureResource(r => r
                    .AddService("LionFire.Test")
                    //.AddService("LionFire.Vos")
                    //.AddService("LionFire.Persistence.Filesystem")
                    //.AddService("LionFire.Vos.RootManager")
                    //.AddService("LionFire.Persisters.SharpZipLib.SharpZipLibExpander")
                    //.AddService("LionFire.Persistence.Handles.WeakHandleRegistry")

                )
                //.AddRuntimeInstrumentation()
                .AddMeter("LionFire.Test")
                .AddMeter("LionFire.Execution.AutoRetry")
                .AddVosInstrumentation()
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
