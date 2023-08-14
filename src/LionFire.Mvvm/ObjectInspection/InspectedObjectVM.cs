namespace LionFire.Inspection;

public class InspectedObjectVM : InspectorNode<IInspectorNodeInfo>, IInspectorNode
{
    public InspectedObjectVM(object sourceObject, ObjectInspectorService objectInspectorService)
    {
        SourceObject = sourceObject;

        //InspectedObjects = GetInspectedObjects(objectInspectorService, sourceObject);

        EffectiveObject = InspectedObjects.LastOrDefault() ?? SourceObject;

        // TODO - lang-ext discriminated unions for MemberVMs?  To handle multiple types while still providing type safety and API discoverability?
        //MemberVMs = ReflectionMemberVM.GetFor(InspectedObject?.EffectiveObject); // TypeModel?.Members.Select(m => MemberVM.Create(m, o)).ToList() ?? new();

    }

    //public IEnumerable<object> GetInspectedObjects(ObjectInspectorService objectInspectorService, object obj)
    //{
    //    foreach (var oi in objectInspectorService.ObjectInspectors)
    //    {
    //        foreach (var item in oi.GetInspectedObjects(obj))
    //        {
    //            yield return item;
    //        }
    //    }
    //}

    public object SourceObject { get; set; }

    public IEnumerable<object> InspectedObjects { get; }

    [Reactive]
    public object EffectiveObject { get; set; }

    public IEnumerable<IInspectorNode> Members { get; set; } = Enumerable.Empty<ReflectionMemberVM>();

}
