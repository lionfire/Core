using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System.Reactive.Linq;

namespace LionFire.Inspection.Nodes;

public abstract class Node : ReactiveObject, INode
{
    #region Identity

    public string Key { get; init; }
    public IEnumerable<string> PathChunks => Parent?.PathChunks.Append(Key) ?? new[] { Key };

    #endregion

    #region Relationships

    /// <summary>
    /// Mutable
    /// </summary>
    [Mutable]
    public object? Source
    {
        get => SourceNode == null ? source : SourceNode.Source;
        set => this.RaiseAndSetIfChanged(ref source, value);
    }
    private object? source;

    public INode SourceNode { get; init; }
    public bool HasSourceNode => !ReferenceEquals(SourceNode, this);

    #endregion

    #region Lifecycle

    protected Node(INode? parent, INode sourceNode, InspectorContext? inspectorContext = null) : this(parent, source: null, inspectorContext, sourceNode: sourceNode) { }
    protected Node(INode? parent, object source, InspectorContext? inspectorContext = null) : this(parent, source, inspectorContext, sourceNode: null) { }

    private Node(INode? parent, object? source, InspectorContext? inspectorContext = null, INode? sourceNode = null)
    {
        Source = source;
        SourceNode = sourceNode ?? this;
        Parent = parent;
        Context = inspectorContext;

        this.WhenAnyValue(x => x.Source).Subscribe(source =>
        {
            var newSourceType = source?.GetType() ?? typeof(DBNull);
            if (SourceType != newSourceType) { SourceType = newSourceType; }
        });

        hasChildren = this.WhenAnyValue(x => x.Groups)
            .Select(_ =>
            {
                //if (Children.Count > 0) return (bool?)true;
                if (Groups.Count == 0) return (bool?)false;

                // else Groups > 0 && Children == 0
                return Groups.Items.Any(g => !g.HasValue)
                    ? (bool?)null
                    : false;
            });

        Children = Groups
           .Connect()
           .TransformMany(g => new GroupNode(this, g))
           //.TransformMany(g => g.ObservableCache, node => node.Path)
           .AsObservableCache();

        Key = Parent == null ? "root" : Parent.GetKeyForChild(this);
    }

    public virtual string GetKeyForChild(INode node)
    {
        return nextKey++.ToString();
    }
    int nextKey = 0;

    #endregion

    [Reactive]
    public Type SourceType { get; set; } = typeof(DBNull);

    public INode? Parent { get; }

    public abstract INodeInfo Info { get; }
    public InspectorContext? Context { get; set; }

    //public virtual bool FlattenGroup(InspectorGroup group) => true;

    public abstract SourceCache<InspectorGroup, string> Groups { get; }

    public IObservableCache<INode, string> Children { get; init; }

    public IObservable<bool?> HasChildren => hasChildren;
    private IObservable<bool?> hasChildren;

}

public class GroupNode : Node
{
    public GroupNode(INode? parent, InspectorGroup source, InspectorContext? inspectorContext = null) : base(parent, source, inspectorContext)
    {
    }

    public override INodeInfo Info => throw new NotImplementedException();

    public override SourceCache<InspectorGroup, string> Groups => throw new NotImplementedException();
}