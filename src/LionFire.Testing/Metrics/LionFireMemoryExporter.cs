using OpenTelemetry.Exporter;

public class LionFireMemoryExporter<T> : InMemoryExporter<T>
    where T : class
{
    public LionFireMemoryExporter(ICollection<T> items) : base(items) { }
}
