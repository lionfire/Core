namespace LionFire.Mvvm.ObjectInspection;

public class InspectedObjectVM : InspectorNode<IInspectorNodeInfo>, IInspectorNode
{
    public InspectedObjectVM(object sourceObject, ObjectInspectorService objectInspectorService)
    {
        SourceObject = sourceObject;

        InspectedObjects = objectInspectorService.GetInspectedObjects(sourceObject);

        EffectiveObject = InspectedObjects.LastOrDefault() ?? SourceObject;

        // TODO - lang-ext discriminated unions for MemberVMs?  To handle multiple types while still providing type safety and API discoverability?
        //MemberVMs = ReflectionMemberVM.GetFor(InspectedObject?.EffectiveObject); // TypeModel?.Members.Select(m => MemberVM.Create(m, o)).ToList() ?? new();

    }

    public object SourceObject { get; set; }

    public IEnumerable<object> InspectedObjects { get; }

    [Reactive]
    public object EffectiveObject { get; set; }

    public IEnumerable<IInspectorNode> Members { get; set; } = Enumerable.Empty<ReflectionMemberVM>();

}
