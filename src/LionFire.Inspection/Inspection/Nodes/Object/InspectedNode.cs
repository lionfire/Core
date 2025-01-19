
using DynamicData.Binding;
using LionFire.Data.Collections;
using ReactiveUI;
using System.Diagnostics;

namespace LionFire.Inspection.Nodes;

public class InspectedNode : InspectedNode<INodeInfo>
{
    public InspectedNode(object source, InspectorContext context, INode? parent = null, INodeInfo? info = null) : base(source: source, context: context, parent: parent, info: info ?? InspectedNodeInfo.Default)
    {
    }
}

public class InspectedNode<TInfo> : Node<TInfo>, IInspectedNode, IDisposable
    where TInfo : INodeInfo
{
    // Children: a flattened collection of the INodes that have been retrieved from all of the Groups
    // Groups: pluggable

    #region Lifecycle

    public InspectedNode(TInfo info, object source, InspectorContext context)
        : this(info: info, source: source, context: context, parent: null)
    //: base(source, parent: null, info: InspectedNodeInfo.Default, context: context)
    {
    }

    protected InspectedNode(TInfo info, INode? parent = null, object? source = null, string? key = null, InspectorContext? context = null)
        : base(parent: parent, source: source, info: info, key: key, context: context)
    {
        disposable = this.Attach();

        this.WhenAnyValue(n => n.Source).Subscribe(v => OnSourceChanged(v));
        this.WhenAnyValue(n => n.Value).Subscribe(v => OnValueChanged(v));
        this.ObservableForProperty(n => n.Values).Subscribe(v => OnValueChanged(v));
        this.Values.Subscribe(v => OnValueChanged(v));
    }
    
    IDisposable? disposable;

    public void Dispose()
        => Interlocked.CompareExchange(ref disposable, null, null)?.Dispose();

    #endregion

    public SourceCache<IInspectorGroup, string> Groups => groups ??= new(g => g.Info.Key);
    private SourceCache<IInspectorGroup, string>? groups;

    public IAsyncReadOnlyKeyedCollection<string, INode> Children => new PreresolvedAsyncObservableCollection<string, INode>(Groups.AsObservableCache().Connect().Cast(g => (INode)g).AsObservableCache());

}
