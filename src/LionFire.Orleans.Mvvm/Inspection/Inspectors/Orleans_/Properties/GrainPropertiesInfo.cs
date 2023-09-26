using LionFire.ExtensionMethods.Orleans_;
using LionFire.Inspection.Nodes;

namespace LionFire.Inspection;

public class GrainPropertiesInfo : GroupInfo
{
    public const string DefaultName = "Properties";
    public GrainPropertiesInfo() : base("Orleans.Grain.Properties")
    {
        Name = DefaultName;
    }

    public override IInspectorGroup CreateNode(INode node, IInspector? inspector = null)
    {
        return new GrainPropertiesGroup(inspector, node, this);
    }

    public override bool IsTypeSupported(Type sourceType) => sourceType.IsOrleansProxy();
}
