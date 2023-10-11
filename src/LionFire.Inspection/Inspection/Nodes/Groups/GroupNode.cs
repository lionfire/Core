using LionFire.Data.Collections;
using LionFire.IO;
using System.Reactive.Linq;

namespace LionFire.Inspection.Nodes;

public abstract class GroupNode : Node<GroupInfo>, IGroupNode
{
    #region Relationships

    public IInspector Inspector { get; }
    
    #endregion

    #region Lifecycle

    public GroupNode(IInspector inspector, INode parent, GroupInfo info, string? key = null, InspectorContext? inspectorContext = null) : base(parent, source: parent.Source, info, key, inspectorContext)
    {
        ArgumentNullException.ThrowIfNull(parent);
        Inspector = inspector;
    }

    #endregion

    public abstract IAsyncReadOnlyDictionary<string, INode> Children { get; }
}

