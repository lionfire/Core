using OpenTelemetry;
using OpenTelemetry.Metrics;

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
