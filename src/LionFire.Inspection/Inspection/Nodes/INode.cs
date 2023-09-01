
using DynamicData.Binding;
using LionFire.Reactive;
using LionFire.Structures.Keys;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace LionFire.Inspection.Nodes;

/// <remarks>
/// Inheritors:
///  - IInspectedNode: Children consist of GroupNodes.  NodeVM might flatten Groups' Children into a single collection.
///  - IGroupNode: Children are directly provided by the particular GroupNode implementation
/// </remarks>
public interface IHierarchicalNode
    : INode
    //, IKeyProvider<string, INode> // TODO: Eliminate this? Bring it back on individual INode implementations if needed?
{
    IObservableCache<INode, string>? Children { get; }

    IObservable<bool?> HasChildren { get; }
}

/// <summary>
/// Decorates the parent Node with Children
///   - Groups: none (at least that's the plan)
///   - Children: Groups provide them directly for the parent Node

/// </summary>
public interface IGroupNode : IHierarchicalNode
{
}

/// <summary>
/// Groups are writable by IInspectors, and are the intended source for effective Children 
/// </summary>
public interface IInspectedNode : IHierarchicalNode
{
    /// <summary>
    /// A writable blackboard for adding and removing InspectorGroups that are useful for inspecting the Source.
    /// Recommendation:
    ///  - Use ObjectInspectorService.Attach to automatically manage this.  It will use all available IInspectors, and you can create and register your own.
    /// </summary>
    SourceCache<InspectorGroup, string>? Groups { get; }
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
    : IReactiveObjectEx
{
    #region Identity

    INodeInfo Info { get; }

    /// <summary>
    /// A unique key for this node within its parent.  Parents are responsible for generating this via GetKey.
    /// </summary>
    string Key { get; }

    #region Derived

    IEnumerable<string> PathChunks { get; }
    string Path => string.Join('/', PathChunks);

    #endregion

    #endregion

    #region Relationships

    INode? Parent { get; }

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
    object? Source { get; }

    #region Semi-derived

    /// <summary>
    /// Should be set to the base interface or class that is expected for changing values of Source.  This will avoid unnecessary IInspector.Attach() calls from all registered IInspectors.
    /// 
    /// Default behavior: when setting Source, if the new Source is not assignable to SourceType, then SourceType is set to Source.GetType()  
    /// 
    /// Always considered nullable.
    /// </summary>
    /// <remarks>
    /// ENH: a polymorphic root service for configuring the natural root types.
    /// ENH: support both nullable and non-nullable Source values
    /// </remarks>
    Type SourceType { get; set; }

    #endregion

    #endregion

    #region Parameters

    InspectorContext? Context { get; set; } // REVIEW - make non nullable?

    #endregion


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


#if OLD
    //InspectedObjects = GetInspectedObjects(objectInspectorService, sourceObject);
    //EffectiveObject = InspectedObjects.LastOrDefault() ?? SourceObject;
    //public IEnumerable<object> GetInspectedObjects(ObjectInspectorService objectInspectorService, object obj)
    //{
    //    foreach (var oi in objectInspectorService.ObjectInspectors)
    //    {
    //        foreach (var item in oi.GetInspectedObjects(obj))
    //        {
    //            yield return item;
    //        }
    //    }
    //}
    //public IEnumerable<object> InspectedObjects { get; }
    //[Reactive]
    //public object EffectiveObject { get; set; }
#endif



}

