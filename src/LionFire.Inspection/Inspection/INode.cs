
using DynamicData.Binding;
using LionFire.Reactive;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace LionFire.Inspection.Nodes;

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
    string Key { get; }
    IEnumerable<string> PathChunks { get; }
    string Path => string.Join('/', PathChunks);

    string GetKeyForChild(INode node);

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
    object? Source { get; set; }

    INode SourceNode { get; }

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

    INode? Parent { get; }
    INodeInfo Info { get; }

    InspectorContext? Context { get; set; } // REVIEW - make non nullable?

    IObservableCache<INode, string> Children { get; }

    /// <summary>
    /// A writable blackboard for adding and removing InspectorGroups that are useful for inspecting the Source.
    /// Recommendation:
    ///  - Use ObjectInspectorService.Attach to automatically manage this.  It will use all available IInspectors, and you can create and register your own.
    /// </summary>
    SourceCache<InspectorGroup, string> Groups { get; }

    IObservable<bool?> HasChildren { get; }

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

