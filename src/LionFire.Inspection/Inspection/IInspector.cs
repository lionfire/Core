
namespace LionFire.Inspection;

// Examples:
//  - PocoInspector
//  - GrainInspector
//  - VobInspector

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
    /// Assumes node.Source is immutable
    /// If node.Source is a primitive, this is a one shot operation.  If it is a more complex object that changes over time, the inspector may subscribe to events on the Source and add or remove NodeGroups as appropriate.
    /// </summary>
    /// <param name="node"></param>
    void Attach(IInspectorNode node)
    {
        if (node.Source is null) return;

        if(IsSourceSubscribable(node.Source))
        {

        } 
        else
        {
            node.Groups. GroupsForObject(node.Source);
        }

    }

    bool IsSourceSubscribable(object source);

    IEnumerable<IObservableCache<IInspectorNode, string>> GroupsForObject(object @object);

}
