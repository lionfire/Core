using LionFire.Data.Collections;
using LionFire.Inspection.Nodes;

namespace LionFire.Inspection;

public static class InspectorConstants
{
    //public Type NullType => typeof(DBNull); 
    public static Type NullType => typeof(NullType);
}
public static class NullType { }


/// <summary>
/// 
/// </summary>
/// <remarks>
/// Examples:
///  - ReflectionInspector
///  - GrainInspector
///  - VobInspector
/// </remarks>
public interface IInspector
{
    ///// <summary>
    ///// If the object would benefit from being inspected as another object type, create and return that type.
    ///// If that object would also in turn benefit from being inspected as another object type, create that type as well
    ///// </summary>
    ///// <param name="object"></param>
    ///// <returns></returns>
    //IEnumerable<object> GetInspectedObjects(object @object);

    /// <summary>
    /// Take responsibility for adding/removing INode.Groups owned by this IInspector
    /// 
    /// Assumes node.Source is immutable
    /// If node.Source is a primitive, this is a one shot operation.  If it is a more complex object that changes over time, the inspector may subscribe to events on the Source and add or remove NodeGroups as appropriate.
    /// </summary>
    /// <param name="node"></param>
    IDisposable? Attach(IInspectedNode node)
    {
        var source = node.Source;

        Type attachmentType = BaseAttachmentType(source?.GetType() ?? InspectorConstants.NullType); // TODO - Use attachmentType?
        return new AttachedInspector(this, node);
    }

    Type BaseAttachmentType(Type type) => type;

    bool IsSourceTypeSupported(Type sourceType)
    {
        foreach (var gi in GroupInfos.Values)
        {
            if (gi.IsSourceTypeSupported(sourceType)) return true;
        }
        return false;
    }

    IReadOnlyDictionary<string, GroupInfo> GroupInfos { get; }

    //IEnumerable<GroupInfo> EnabledGroups(INode node) => node.Groups.Items.Where(g => GroupInfos.Select(gi => gi.Key).Contains(g.Info.Key));
    //IEnumerable<string> DisabledGroups(INode node) => GroupInfos.Select(gi=>gi.Key).Except(node.Groups.Items.Select(i=>i.Info.Key));
}

public static class IInspectorX
{


}
