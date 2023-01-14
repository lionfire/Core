using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using System.Collections.Generic;

namespace LionFire.Testing;

public static partial class TestRunner
{
    public static Dictionary<string, (Metric metric, MetricPoint metricPoint, object? value)> GetMetrics(IServiceProvider sp, bool log = false)
    {
        var l = log ? sp.GetRequiredService<ILogger<TestLog>>() : null;

        Assert.IsTrue(sp.GetRequiredService<LionFireMetricReader>().DoCollect());

        var list = new List<(Metric metric, MetricPoint metricPoint, object? value)>();

        var metrics = sp.GetService<ICollection<Metric>>()!;
        //var metrics = ActivitiesExport.Metrics.Value!;

        foreach (var metric in metrics)
        {
            MetricPoint? GetMetricPoint(Metric metric)
            {
                if (LionFireEnvironment.MetricsContext != null)
                {
                    foreach (var metricPoint in metric.GetMetricPoints())
                    {
                        foreach (var tag in metricPoint.Tags)
                        {
                            if (tag.Key == LionFireEnvironment.MetricsContextKey && LionFireEnvironment.MetricsContext.Equals(tag.Value))
                            {
                                return metricPoint;
                            }
                        }
                    }
                    // Didn't find matching context, so return first one
                    foreach (var metricPoint in metric.GetMetricPoints())
                    {
                        return metricPoint;
                    }
                }
                else
                {
                    // Commented because if it's 0, we don't want to return the first one from another unit test.
                    //foreach (var metricPoint in metric.GetMetricPoints())
                    //{
                    //    return metricPoint; // return first one
                    //}
                }
                return null;
            }

            var metricPoint = GetMetricPoint(metric);
            if (metricPoint.HasValue)
            {
                object? value = null;

                var metricType = metric.MetricType;
                if (metricType.IsLong())
                {
                    if (metricType.IsSum())
                    {
                        value = metricPoint.Value.GetSumLong();
                    }
                    else
                    {
                        value = metricPoint.Value.GetGaugeLastValueLong();
                    }
                }
                list.Add((metric, metricPoint.Value, value));
            }

        }

        Dictionary<string, (Metric metric, MetricPoint metricPoint, object? value)> result;
        try
        {
            result = list.ToDictionary(x => $"{x.metric.MeterName}.{x.metric.Name}");
        }
        catch (Exception ex)
        {

            throw new Exception($"CollectCount: {sp.GetRequiredService<LionFireMetricReader>().CollectCount}. Duplicate meter keys: " + list.GroupBy(i => i.metric.MeterName).Where(g => g.Count() > 1).Select(g => g.Key).AggregateOrDefault((x, y) => $"{x}, {y}"), ex);
        }

        if (log)
        {
            l.LogInformation("Metrics:" + Environment.NewLine + DumpMetrics(result).ToString());
        }
        return result;
    }
    public static StringBuilder DumpMetrics(Dictionary<string, (Metric metric, MetricPoint metricPoint, object? value)> dict)
    {
        var sb = new StringBuilder();

        foreach (var x in dict)
        {
            sb.AppendLine($" - {x.Key}: {x.Value.value?.ToString() ?? "?"}");
            //System.Console.WriteLine($"OTel: {item.Name}: {item.Dump()}");
        }
        return sb;
    }
    public static void GenerateAsserts(Dictionary<string, (Metric metric, MetricPoint metricPoint, object? value)> dict)
    {
        Console.WriteLine();
        Console.WriteLine("#region Metrics");
        Console.WriteLine();
        Console.WriteLine("var metrics = GetMetrics(sp, log: true);");
        
        foreach (var x in dict.Where(i => !i.Value.metric.Name.EndsWith(".") && i.Value.metric.MetricType != MetricType.LongGauge))
        {
            // TODO: cast type
            Console.WriteLine($"Assert.AreEqual({x.Value.value}, (long)metrics[\"{x.Key}\"].value!);");
            //System.Console.WriteLine($"OTel: {item.Name}: {item.Dump()}");
        }
        Console.WriteLine("TestRunner.RanAsserts = true;");
        Console.WriteLine();
        Console.WriteLine("#endregion");
        Console.WriteLine();
        //Console.WriteLine($"Assert.AreEqual({dict.Count}, metrics.Count);");

    }
    public static bool RanAsserts { get; set; }
}