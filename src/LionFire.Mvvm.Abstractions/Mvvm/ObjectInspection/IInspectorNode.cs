using LionFire.Data.Async.Gets;
using DynamicData;
using DynamicData.Cache;
using DynamicData.Cache.Internal;
using System.Reactive.Linq;

namespace LionFire.Mvvm.ObjectInspection;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
public interface IInspectorMember
{
    IInspectorMemberInfo? Info { get; }

    /// <summary>
    /// sync: object
    /// async: IGetter, ISetter, IValue
    /// </summary>
    object Source { get; }
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
public interface IInspectorNode
    : IInspectorMember
    , IReactiveNotifyPropertyChanged<IReactiveObject>
    , IHandleObservableErrors
    , IReactiveObject
{
    #region Derived inspectors, Derived from

    /// <summary>
    /// If set, this Inspector Node is a more sophisticated way to inspect another node.
    /// </summary>
    IEnumerable<IInspectorNode> DerivedFrom { get; }

    /// <summary>
    /// If set, more sophisticated inspectors are available and recommended
    /// </summary>
    IReadOnlyDictionary<string, IInspectorNode> DerivedInspectors { get; }

    #endregion

    /// <summary>
    ///  - collections: items
    ///  - objects: members
    /// </summary>
    /// <remarks>
    /// Default ordering: 
    ///  - by group depth
    ///  - by group order
    ///  - by node order
    /// </remarks>
    IObservableList<(IInspectorNode node, InspectionGroupGetter groupGetter)> Children { get => Groups.Connect().ToCollection()
            .SelectMany(groups => groups.SelectMany(group => (group.WhenAnyValue(g=>g.Value), group.Info))//.Value.Select(v => (g.Key, v))))
        //.ToObservableList(); 

} // REVIEW: IObservableList<IInspectorNode, int> instead?
            //.ToSortedCollection(,).OrderByDescending(v => v.Info.Depth).ThenBy(v => v.Info.Order).ThenBy(v => v.Info.Key); } // REVIEW: IObservableList<IInspectorNode, int> instead?

    IObservableCache<InspectionGroupGetter, string> Groups { get; }

}

public class InspectorGroupInfo : IKeyable<string>
{

    #region Relationships

    public string? Parent { get; set; }

    #region Derived

    /// <summary>
    /// How many parents this group has.  0 means it is a root group.  The higher the number, the more likely it is that the group is a more sophisticated and tailored way to inspect the object.
    /// </summary>
    public int Depth
    {
        get
        {
            int depth = 0;
            var p = Parent;
            while (p != null)
            {
                depth++;
                p = Parent;
            }
            return depth;
        }
    }

    #endregion

    #endregion

    #region Identity

    public string Key { get; set; }

    #endregion

    #region Lifecycle

    public InspectorGroupInfo(string key)
    {
        Key = key;
    }

    #endregion

    #region Properties

    public string Key { get; set; }
    public string DisplayName { get; set; }

    public float Order { get; set; }

    #endregion
}

public class InspectionGroupGetter : GetterRxO<IEnumerable<IInspectorNode>>,  IGetter<IEnumerable<IInspectorNode>> // TODO: Change to collection, with TValue IObservableList<IInspectorNode>
{
    public InspectorGroupInfo Info { get; set; }


}

//public class InspectorChildrenInfo
//{
//    public InspectorChildKind Kind { get; set; }

//}
//public class InspectorChildrenVM
//{

//}

public enum InspectorChildKind
{
    Unspecified = 0,
    Member = 1 << 0,
    Item = 1 << 1,
    Relationship = 1 << 2,
}

//public interface IInspectorMemberVM<TSource> : IInspectorMemberVM // not needed since ObjectInspection primarily works at runtime without static typing
//{
//    TSource Source { get; }
//}
