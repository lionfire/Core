namespace LionFire.Inspection;

public class InspectorContext
{
    public IServiceProvider ServiceProvider { get; }
    public ObjectInspectorService ObjectInspectorService { get; }

    public InspectorContext(IServiceProvider serviceProvider, ObjectInspectorService objectInspectorService)
    {
        ServiceProvider = serviceProvider;
        ObjectInspectorService = objectInspectorService;
    }
}
