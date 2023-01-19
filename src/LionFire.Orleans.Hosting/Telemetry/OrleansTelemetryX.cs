using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace LionFire.Hosting;

public static class OrleansTelemetryX
{
    // See https://github.com/ReubenBond/orleans/blob/sample/otel/samples/GPSTracker/GPSTracker.Service/Program.cs

    public static MeterProviderBuilder AddOrleans(this MeterProviderBuilder builder)
        => builder.AddMeter("Microsoft.Orleans");

    public static TracerProviderBuilder AddOrleans(this TracerProviderBuilder builder)
        => builder
            .AddSource("Microsoft.Orleans.Runtime")
            .AddSource("Microsoft.Orleans.Application");
}
