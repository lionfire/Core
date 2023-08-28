
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace LionFire.Inspection.Nodes;

public class SummaryNode : Node, INode
{
    public SummaryNode(INode? parent, INode sourceNode, INodeInfo info) : base(parent, sourceNode)
    {
        Info = info;
    }

    public override INodeInfo Info { get; }
        

    public override SourceCache<InspectorGroup, string> Groups => throw new NotImplementedException();
}


public class SummaryNodeInfo : NodeInfo
{
    public SummaryNodeInfo()
    {
        NodeKind = InspectorNodeKind.Summary;
    }

    //#region Dependencies

    //public IServiceProvider ServiceProvider { get; }

    //#endregion

    //#region Lifecycle

    //public ValueSummaryNodeInfo(IServiceProvider serviceProvider)
    //{
    //    ServiceProvider = serviceProvider;
    //}

    //#endregion

    //public INode CreateNode(object source)
    //{
    //    return new ValueSummaryNode(source, null);
    //}
}
