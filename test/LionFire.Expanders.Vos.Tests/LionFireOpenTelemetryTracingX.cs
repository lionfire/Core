using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

public static class LionFireOpenTelemetryTracingX // MOVE
{
    public static IServiceCollection AddOpenTelemetryTracingLF(this IServiceCollection services, string? serviceName = null, string? serviceVersion = null, Action<TracerProviderBuilder>? configure = null)
    {
        services
        .AddOpenTelemetry()
            .WithMetrics(c => c
                .AddConsoleExporter(o => o
                .Targets = OpenTelemetry.Exporter.ConsoleExporterOutputTargets.Console
                )
                .AddInMemoryExporter(ActivitiesExport.Metrics.Value ??= new(), c=>c
                .PeriodicExportingMetricReaderOptions = new PeriodicExportingMetricReaderOptions
                {
                    ExportIntervalMilliseconds = 1000,
                    //ExportTimeoutMilliseconds
                })
                //.AddOtlpExporter(opts =>
                //{
                //    opts.Endpoint = new Uri(builder.Configuration["Otlp:Endpoint"]);
                //})

                .ConfigureResource(r => r
                    .AddService("LionFire.Test")
                    .AddService("LionFire.Vos")
                )
                //.AddRuntimeInstrumentation()
                .AddMeter("LionFire.Test")
                .AddMeter("LionFire.Vos")
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
