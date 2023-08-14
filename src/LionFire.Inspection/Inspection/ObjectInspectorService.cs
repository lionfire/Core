namespace LionFire.Inspection;

public class ObjectInspectorService
{
    public ObjectInspectorService(IEnumerable<IInspector> inspectors)
    {
        Inspectors = inspectors;
    }

    public IEnumerable<IInspector> Inspectors { get; }

    //public IObservableList<IObjectInspector> ObservableObjectInspectors { get; } // ENH, for dynamic info environments

}

public static class ObjectInspectorServiceX
{
    public static void Inject(this ObjectInspectorService objectInspectorService, IInspectorNode node)
    {

        foreach(var inspector in objectInspectorService.Inspectors)
        {

        }
    }
}