namespace LionFire.Mvvm.ObjectInspection;

public class ObjectInspectorService
{
    public ObjectInspectorService(IEnumerable<IObjectInspector> objectInspectors)
    {
        ObjectInspectors = objectInspectors;
    }

    public IEnumerable<IObjectInspector> ObjectInspectors { get; }

    public IEnumerable<InspectedObjectItem> GetInspectedObjects(object obj)
    {
        foreach (var oi in ObjectInspectors)
        {
            foreach (var item in oi.GetInspectedObjects(obj))
            {
                yield return item;
            }
        }
    }
}
