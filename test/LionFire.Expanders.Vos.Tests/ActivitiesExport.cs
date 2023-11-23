using System.Diagnostics;
using OpenTelemetry.Metrics;

public static class ActivitiesExport
{
    public static AsyncLocal<List<Activity>> Activities = new();
    public static AsyncLocal<List<Metric>> Metrics = new();
}
