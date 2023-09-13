
using DynamicData.Binding;
using LionFire.Data.Collections;

namespace LionFire.Inspection.Nodes;

public class InspectedNode : Node<INodeInfo>, IInspectedNode, IDisposable
{
    // Children: a flattened collection of the INodes that have been retrieved from all of the Groups
    // Groups: pluggable

    #region Lifecycle
        
    public InspectedNode(object source, InspectorContext context)
        : this(source, context, parent: null)
    //: base(source, parent: null, info: InspectedNodeInfo.Default, context: context)
    {
    }

    public InspectedNode(object source, InspectorContext context, INode? parent=null, INodeInfo? info = null)
        : base(parent, source, info: info ?? InspectedNodeInfo.Default, context: context)
    {
        disposable = this.Attach();
    }

    IDisposable? disposable;

    public void Dispose() 
        => Interlocked.CompareExchange(ref disposable, null, null)?.Dispose();

    #endregion

    public SourceCache<IInspectorGroup, string> Groups => groups ??= new(g => g.Info.Key);
    private SourceCache<IInspectorGroup, string>? groups;

    public IAsyncReadOnlyDictionary<string, INode> Children => new PreresolvedAsyncObservableCollection<string, INode>(Groups.AsObservableCache().Connect().Cast(g => (INode)g).AsObservableCache());

    
}
