using LionFire.Persistence;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using OpenTelemetry.Metrics;
using System.Diagnostics;

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

        _Run(hostApplicationFactory, (h, testName) =>
        {
            //LionFireEnvironment.MetricsContext = testName;
            LionFireEnvironment.ContextTags = new KeyValuePair<string, object?>[]
            {
                new KeyValuePair<string, object?>("test_name", testName),
                new KeyValuePair<string, object?>("test_start", DateTimeOffset.Now),
            };

            configure?.Invoke(h);

            h.Services.AddOpenTelemetryTracingLF();

            h.Run(async sp =>
            {

                Dictionary<string, (Metric metric, MetricPoint metricPoint, object? value)>? metrics = null;

                try
                {
                    await a(sp);
                }
                catch (AssertFailedException)
                {
                    #region Generate metric assertions, if they were not accessed
                    if (sp.GetRequiredService<LionFireMetricReader>().CollectCount == 0)
                    {
                        metrics ??= GetMetrics(sp, log: true);
                        GenerateAsserts(metrics);
                    }
                    #endregion
                    throw;
                }

                #region Generate metric assertions, if they were not accessed
                if (!TestRunner.RanAsserts)
                {
                    if (sp.GetRequiredService<LionFireMetricReader>().CollectCount == 0)
                    {
                        metrics ??= GetMetrics(sp, log: true);
                        if (metrics.Count > 0)
                        {
                            GenerateAsserts(metrics);
                        }
                    }
                }
                #endregion
            });
        });
    }

    public static void RunTest(Func<IServiceProvider, Task> a, Func<HostApplicationBuilder>? hostApplicationFactory = null)
    {
        RunTest(null, a, hostApplicationFactory);
    }

    public static void RunTest(Action<IServiceProvider> a, Func<HostApplicationBuilder>? hostApplicationFactory = null)
    {
        LionFireEnvironment.IsMultiApplicationEnvironment = true;
        //var testMethodName = LionFireEnvironment.TestMethodName;

        //var H = (hostApplicationFactory ?? HostApplicationFactory ?? throw new ArgumentNullException($"Set either {nameof(hostApplicationFactory)} or {typeof(TestRunner).FullName}.{nameof(HostApplicationFactory)}")).Invoke();

        //H.Run(sp =>
        //{
        //    LionFireEnvironment.MetricsContext = testMethodName;
        //    a(sp);
        //});
        //Serilog.Log.CloseAndFlush();


        _Run(hostApplicationFactory, (h, testName) =>
        {
            h.Run(sp =>
            {
                LionFireEnvironment.ContextTags = new KeyValuePair<string, object?>[]
                {
                    new KeyValuePair<string, object?>("test_name", testName),
                    new KeyValuePair<string, object?>("test_start", DateTimeOffset.Now),
                };
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