namespace LionFire.Inspection.Nodes;

public class InspectedNodeInfo : NodeInfoBase
{
    public static InspectedNodeInfo Default { get; } = new InspectedNodeInfo();

    #region Lifecycle

    public InspectedNodeInfo()
    {
        Name = "(inspected object)";
        Type = typeof(object);
    }

    #endregion

    public override InspectorNodeKind NodeKind => InspectorNodeKind.Object;
}