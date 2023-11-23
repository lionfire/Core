using LionFire.Data.Collections;

namespace LionFire.Inspection.Nodes;

public abstract class SyncFrozenGroup : FrozenGroup
{

    protected SyncFrozenGroup(IInspector inspector, INode parent, GroupInfo info, string? key = null, InspectorContext? inspectorContext = null) : base(inspector, parent, info, key, inspectorContext)
    {
        Children = new OneShotSyncDictionary<string, INode>(GetChildren);
    }

    public override IAsyncReadOnlyDictionary<string, INode> Children { get; }

    //public override IDictionary<string, INode>? Value => throw new NotImplementedException();

    protected abstract IEnumerable<KeyValuePair<string, INode>> GetChildren();
}

