using OpenTelemetry.Metrics;

namespace LionFire.Testing;

public static partial class TestRunner
{
    public static Func<HostApplicationBuilder>? HostApplicationFactory { get; set; }
    public static Func<IHostBuilder>? HostFactory { get; set; }

    public static void RunTest(Action<HostApplicationBuilder>? configure, Func<IServiceProvider, Task> a, Func<HostApplicationBuilder>? hostApplicationFactory = null)
    {
        //var testMethodName = LionFireEnvironment.TestMethodName;

        //var H = (hostApplicationFactory ?? HostApplicationFactory ?? throw new ArgumentNullException($"Set either {nameof(hostApplicationFactory)} or {typeof(TestRunner).FullName}.{nameof(HostApplicationFactory)}")).Invoke();

        //H.Run(async sp =>
        //{
        //    LionFireEnvironment.MetricsContext = testMethodName;
        //    await a(sp);
        //});
        //Serilog.Log.CloseAndFlush();

        _Run(hostApplicationFactory, (h, metricsContext) =>
        {
            configure?.Invoke(h);
            
            h.Services.AddOpenTelemetryTracingLF();

            h.Run(async sp =>
            {
                LionFireEnvironment.MetricsContext = metricsContext;

                try
                {
                    await a(sp);
                }
                catch (AssertFailedException)
                {
                    var metrics = GetMetrics(sp, log: true);
                    GenerateAsserts(metrics);
                    throw;
                }

                if (!TestRunner.RanAsserts)
                {
                    var metrics = GetMetrics(sp, log: true);
                    if (metrics.Count > 0)
                    {
                        GenerateAsserts(metrics);
                    }
                }
            });
        });
    }

    public static void RunTest(Func<IServiceProvider, Task> a, Func<HostApplicationBuilder>? hostApplicationFactory = null)
    {
        RunTest(null, a, hostApplicationFactory);
    }

    public static void RunTest(Action<IServiceProvider> a, Func<HostApplicationBuilder>? hostApplicationFactory = null)
    {
        //var testMethodName = LionFireEnvironment.TestMethodName;

        //var H = (hostApplicationFactory ?? HostApplicationFactory ?? throw new ArgumentNullException($"Set either {nameof(hostApplicationFactory)} or {typeof(TestRunner).FullName}.{nameof(HostApplicationFactory)}")).Invoke();

        //H.Run(sp =>
        //{
        //    LionFireEnvironment.MetricsContext = testMethodName;
        //    a(sp);
        //});
        //Serilog.Log.CloseAndFlush();


        _Run(hostApplicationFactory, (h, metricsContext) =>
        {
            h.Run(sp =>
            {
                LionFireEnvironment.MetricsContext = metricsContext;
                a(sp);
            });
        });
    }

    private static void _Run(Func<HostApplicationBuilder>? hostApplicationFactory, Action<HostApplicationBuilder, string> run)
    {
        var testMethodName = LionFireEnvironment.TestMethodName;

        var H = (hostApplicationFactory ?? HostApplicationFactory ?? throw new ArgumentNullException($"Set either {nameof(hostApplicationFactory)} or {typeof(TestRunner).FullName}.{nameof(HostApplicationFactory)}")).Invoke();

        run(H, testMethodName);

        Serilog.Log.CloseAndFlush();
    }
}