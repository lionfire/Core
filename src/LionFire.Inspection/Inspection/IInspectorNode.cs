
using DynamicData.Binding;
using LionFire.Data.Async.Gets;
using LionFire.Data.Async.Sets;
using LionFire.Reactive;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace LionFire.Inspection.Nodes;

public class ObjectNodeInfo : INodeInfo
{

    #region Identity

    public string Name => throw new NotImplementedException();

    public InspectorNodeKind NodeKind => InspectorNodeKind.Object;

    #endregion

    #region Dependencies

    public IServiceProvider ServiceProvider { get; }

    #endregion

    #region Lifecycle

    public ObjectNodeInfo(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    #endregion

    public IEnumerable<string> Flags { get { yield return break; } }

    public string? Order => null;

    public INode CreateNode(object source)
    {
        return new ObjectNode(source, null);
    }

}

public class ObjectNode : INode
{
    // Children: a flattened collection of the INodes that have been retrieved from all of the Groups
    // Groups: pluggable

    public ObjectNode(object source, INode? parent)
    {
        Source = source;
        Parent = parent;
    }

    public object Source { get; }

    public INode? Parent { get; }

    public INodeInfo Info => throw new NotImplementedException();

    public IObservableCollection<INode> Children => throw new NotImplementedException();

    public SourceCache<InspectorGroupGetter, string> Groups => throw new NotImplementedException();
}

public class ValueNode
    : INode
    , IReactiveObjectEx
{
    // Children: ValueObjectNode 
    // Groups: just one group, to retrieve the one IObjectNode 

    IObjectNode ValueObjectNode { get; }


    // A single 
    SourceCache<IGetter<INodeGroup>, string> Groups { get; }
}

public class CollectionNode : INode
{
    // Children: the child items
    // Groups:
    //   - the primary Collection group
}

public interface IGettableNode
{
    IGetter<object> Getter { get; }
}

public interface ISettableNode
{
    ISetter<object> Setter { get; }
}



/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// Source:
///  - POCO:
///    - Info: null
/// </remarks>
public interface INode
//: IReactiveNotifyPropertyChanged<IReactiveObject>
//, IHandleObservableErrors
//, IReactiveObject
{

    /// <summary>
    /// Immutable (REVIEW - trying fo rit)
    /// 
    /// set directly for:
    ///  - IObjectNode
    ///  
    /// derived from Parent for:
    ///  - IValueNode
    ///  - ICollectionNode
    /// could be:
    /// - POCO
    /// - IGetter<T>
    /// - IValue<T>
    /// </summary>
    object Source { get; }

    INode? Parent { get; }
    INodeInfo Info { get; }

    IObservableCollection<INode> Children { get; }

    /// <summary>
    /// A writable blackboard for adding and removing InspectorGroups that are useful for inspecting the Source.
    /// Recommendation:
    ///  - Use ObjectInspectorService.Attach to automatically manage this.  It will use all available IInspectors, and you can create and register your own.
    /// </summary>
    SourceCache<InspectorGroupGetter, string> Groups { get; }

    //#region Derived inspectors, Derived from

    ///// <summary>
    ///// If set, this Inspector Node is a more sophisticated way to inspect another node.
    ///// </summary>
    //IEnumerable<INode> DerivedFrom { get; }

    ///// <summary>
    ///// If set, more sophisticated inspectors are available and recommended
    ///// </summary>
    //IReadOnlyDictionary<string, INode> DerivedInspectors { get; }

    //#endregion

    //    /// <summary>
    //    ///  - collections: items
    //    ///  - objects: members
    //    /// </summary>
    //    /// <remarks>
    //    /// Default ordering: 
    //    ///  - by group depth
    //    ///  - by group order
    //    ///  - by node order
    //    /// </remarks>
    //    IObservableList<(INode node, InspectionGroupGetter groupGetter)> ChildGroups { get => Groups.Connect().ToCollection()
    //            .SelectMany(groups => groups.SelectMany(group => (group.WhenAnyValue(g=>g.Value), group.Info))//.Value.Select(v => (g.Key, v))))
    //        //.ToObservableList(); 

    //} // REVIEW: IObservableList<INode, int> instead?
    //            //.ToSortedCollection(,).OrderByDescending(v => v.Info.Depth).ThenBy(v => v.Info.Order).ThenBy(v => v.Info.Key); } // REVIEW: IObservableList<INode, int> instead?



}

public interface INodeGroup
{
    string Key { get; }
    IEnumerable<INode> Nodes { get; }
}