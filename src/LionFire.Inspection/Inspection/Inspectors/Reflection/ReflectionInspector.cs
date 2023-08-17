using DynamicData.Binding;
using LionFire.Data.Async.Gets;
using LionFire.Data.Collections;
using LionFire.Data.Mvvm;
using System.Reflection;

namespace LionFire.Inspection.Nodes;


public class PropertyNode : GetterRxO<object>, INode
{
    public PropertyNode(object source, PropertyInfo propertyInfo)
    {
        Source = source;
        PropertyInfo = propertyInfo;
    }

    public object Source { get; }
    public PropertyInfo PropertyInfo { get; }

    public SourceCache<IGetter<INodeGroup>, string> Groups => throw new NotImplementedException();

    public INode? Parent => throw new NotImplementedException();

    public INodeInfo Info => throw new NotImplementedException();

    public IObservableCollection<INode> Children => throw new NotImplementedException();

    SourceCache<InspectorGroupGetter, string> INode.Groups => throw new NotImplementedException();
}

public class PublicPropertiesGetter : XGetter
{
    public override IEnumerable<INode> GetValue()
    {
        if (Source == null) yield break;

        foreach (var mi in Source.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
        {
            yield return new InspectorNode(mi.Name, mi.GetValue(Source));
        }
        Source = null;
    }
}

internal class XGetter : SynchronousOneShotGetter<IEnumerable<INode>>
{
    public override string Key => "reflection:Data/Properties/Public";

    protected object Source { get; }

    public PublicPropertiesGetter(object source)
    {
        Source = source;
    }    
}

public class PublicFieldsGetter : OneShotInspectorGroupGetter
{
    public override string Key => "reflection:Data/Fields/Public";
}

public class ReflectionInspectionGroups
{
    public InspectorGroupGetter PropertyMembers 
    { 
    
    }
}


public class ReflectionInspectorGroupGetter : IEnumerableGetter<IObservableCache<INode, string>>
{
    ConcurrentDictionary<Type, IDictionary<string, >>
    static ReflectionInspectorGroupGetter Get(object source)
    {
        var type = source.GetType();
    }
}


public class ReflectionInspector : IInspector
{
    public IEnumerableGetter<IObservableCache<INode, string>> GroupsForObject(object source)
    {
        return new ReflectionInspectorGroupGetter(source);
    }

    public bool IsSourceSubscribable(object source)
    {
        throw new NotImplementedException();
    }

}

public class GrainInspector : IInspector
{
    public IEnumerableGetter<IObservableCache<INode, string>> GroupsForObject(object source)
    {
        throw new NotImplementedException();
    }

    public bool IsSourceSubscribable(object source)
    {
        throw new NotImplementedException();
    }

}

//public class FlexInspector : IInspector
//{
//}

//public class VobInspector : IInspector
//{    
//}
