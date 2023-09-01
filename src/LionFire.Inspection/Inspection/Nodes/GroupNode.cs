using LionFire.IO;

namespace LionFire.Inspection.Nodes;

public abstract class GroupNode : Node<GroupInfo>, IHierarchicalNode
{
    #region Lifecycle

    public GroupNode(INode? parent, string? key = null ?, InspectorContext? inspectorContext = null) : base(parent, source, key, inspectorContext)
    {
        //source
        //PopulateIntoChildren(source.Nodes, source.Id + "/");
    }

    #endregion

    public IObservableCache<INode, string>? Children => throw new NotImplementedException();

    public IObservable<bool?> HasChildren => throw new NotImplementedException();


    //IObservableCache<INode, string> GroupNodes { get; }

}

