namespace LionFire.Mvvm.ObjectInspection;

public class InspectedObjectVM : ReactiveObject
{
    public InspectedObjectVM(object sourceObject, ObjectInspectorService objectInspectorService)
    {
        SourceObject = sourceObject;

        InspectedObjects = objectInspectorService.GetInspectedObjects(sourceObject);

        EffectiveObject = InspectedObjects.LastOrDefault() ?? SourceObject;
    }

    public object SourceObject { get; set; }

    public IEnumerable<InspectedObjectItem> InspectedObjects { get; }

    [Reactive]
    public object EffectiveObject { get; set; }

}
