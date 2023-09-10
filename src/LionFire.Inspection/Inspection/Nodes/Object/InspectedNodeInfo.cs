namespace LionFire.Inspection.Nodes;

public class InspectedNodeInfo : NodeInfo
{
    public static InspectedNodeInfo Default { get; } = new InspectedNodeInfo();

    #region Lifecycle

    public InspectedNodeInfo()
    {
        Name = "(custom object)";
        NodeKind = InspectorNodeKind.Object;
    }

    #endregion
}