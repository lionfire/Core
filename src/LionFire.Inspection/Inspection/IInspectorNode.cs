
using LionFire.Data.Async.Gets;

namespace LionFire.Inspection;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// Source:
///  - POCO:
///    - Info: null
/// </remarks>
public interface IInspectorNode
//: IReactiveNotifyPropertyChanged<IReactiveObject>
//, IHandleObservableErrors
//, IReactiveObject
{
    //#region Derived inspectors, Derived from

    ///// <summary>
    ///// If set, this Inspector Node is a more sophisticated way to inspect another node.
    ///// </summary>
    //IEnumerable<IInspectorNode> DerivedFrom { get; }

    ///// <summary>
    ///// If set, more sophisticated inspectors are available and recommended
    ///// </summary>
    //IReadOnlyDictionary<string, IInspectorNode> DerivedInspectors { get; }

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
    //    IObservableList<(IInspectorNode node, InspectionGroupGetter groupGetter)> ChildGroups { get => Groups.Connect().ToCollection()
    //            .SelectMany(groups => groups.SelectMany(group => (group.WhenAnyValue(g=>g.Value), group.Info))//.Value.Select(v => (g.Key, v))))
    //        //.ToObservableList(); 

    //} // REVIEW: IObservableList<IInspectorNode, int> instead?
    //            //.ToSortedCollection(,).OrderByDescending(v => v.Info.Depth).ThenBy(v => v.Info.Order).ThenBy(v => v.Info.Key); } // REVIEW: IObservableList<IInspectorNode, int> instead?
        
    /// <summary>
    /// A writable blackboard for adding and removing InspectorGroups that are useful for inspecting the Source.
    /// Recommendation:
    ///  - Use ObjectInspectorService.Attach to automatically manage this.  It will use all available IInspectors, and you can create and register your own.
    /// </summary>
    SourceCache<IGetter<IInspectorNodeGroup>, string> Groups { get; }

    /// <summary>
    /// Immutable (REVIEW - trying fo rit)
    /// 
    /// could be:
    /// - POCO
    /// - IGetter<T>
    /// - IValue<T>
    /// </summary>
    object Source { get; }
}

public interface IInspectorNodeGroup
{
    string Key { get; }
    IEnumerable<IInspectorNode> Nodes { get; }
}