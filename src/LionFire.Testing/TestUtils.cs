using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using System.Collections.Generic;

namespace LionFire.Testing;

public static class TestUtils
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
            foreach (var metricPoint in metric.GetMetricPoints())
            {
                object? value = null;

                var metricType = metric.MetricType;
                if (metricType.IsLong())
                {
                    if (metricType.IsSum())
                    {
                        value = metricPoint.GetSumLong();
                    }
                    else
                    {
                        value = metricPoint.GetGaugeLastValueLong();
                    }
                }
                list.Add((metric, metricPoint, value));

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
        foreach (var x in dict)
        {
            // TODO: cast type
            Console.WriteLine($"Assert.AreEqual({x.Value.value}, (long)metrics[\"{x.Key}\"].value!);");
            //System.Console.WriteLine($"OTel: {item.Name}: {item.Dump()}");
        }
        Console.WriteLine($"Assert.AreEqual({dict.Count}, metrics.Count);");
    }
}