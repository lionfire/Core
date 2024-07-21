using LionFire.Reactive;
using LionFire.Structures;
using ReactiveUI;
using System.Reactive.Linq;

namespace LionFire.Inspection.Nodes;

/// <summary>
/// Decorates the parent Node with Children
///   - Groups: none (at least that's the plan)
///   - Children: Groups provide them directly for the parent Node
/// </summary>
public interface IGroupNode : IHierarchicalNode { }

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
    , IParented<INode> // Immutable
    , IKeyed<string>
{
    #region Identity

    [Immutable]
    INodeInfo Info { get; }

    ///// <summary>
    ///// A unique key for this node within its parent.  Parents are responsible for generating this via GetKey.
    ///// </summary>
    //[Immutable]
    //string Key { get; }

    #region Derived

    IEnumerable<string> PathChunks { get; }
    string Path => string.Join('/', PathChunks);

    #endregion

    #endregion

    #region Relationships

    /// <summary>
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
    [Immutable]
    object? Source { get; }

    IReactiveNotifyPropertyChanged<INode> SourceChangeEvents { get; }
    ReactiveNotifyPropertyChanged<INode> RaiseSourceChangeEvents { get; }

    object? Value { get; set; }
    Type ValueType { get; }
    IObservable<object?> Values { get; }

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
    
    InspectorVisibility Visibility { get; }

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
