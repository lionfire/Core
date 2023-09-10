
using DynamicData.Binding;
using LionFire.Data.Collections;

namespace LionFire.Inspection.Nodes;

public class InspectedNode : Node<INodeInfo>, IInspectedNode
{
    // Children: a flattened collection of the INodes that have been retrieved from all of the Groups
    // Groups: pluggable

    public InspectedNode(object source, InspectorContext context) : base(null, source, info: InspectedNodeInfo.Default, context: context)
    {
    }

    public InspectedNode(INode? parent, object source, INodeInfo info, InspectorContext? context = null) : base(parent, source, info: info, context: context)
    {
    }

    public SourceCache<IInspectorGroup, string> Groups => groups ??= new(g => g.Info.Key);
    private SourceCache<IInspectorGroup, string>? groups;

    public IAsyncReadOnlyDictionary<string, INode> Children => new PreresolvedAsyncObservableCollection<string, INode>(Groups.AsObservableCache().Connect().Cast(g => (INode)g).AsObservableCache());
}

//public class Tzsfgad : AsyncReadOnlyDictionary<string, INode>
//{
//    protected override ITask<IGetResult<IEnumerable<KeyValuePair<string, INode>>>> GetImpl(CancellationToken cancellationToken = default)
//    {
//        throw new NotImplementedException();
//    }
//}