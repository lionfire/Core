using DynamicData.Binding;
using LionFire.Collections;
using LionFire.Data;
using LionFire.Data.Collections;
using LionFire.Data.Mvvm;
using LionFire.Inspection.Nodes;
using MorseCode.ITask;
using System.Collections.Concurrent;
//using LionFire.ExtensionMethods.Collections;

namespace LionFire.Inspection;

public class ReflectionInspector : IInspector
{
    public IReadOnlyDictionary<string, GroupInfo> GroupInfos => groupInfos;
    protected Dictionary<string, GroupInfo> groupInfos = new();

    public bool IsSourceTypeSupported(object source) => source != null;

    public ReflectionInspector()
    {
        groupInfos.Add(PropertiesGroup.GroupInfo.Key, PropertiesGroup.GroupInfo);
        //groupInfos.Add(FieldsGetter.GroupInfo); // TODO NEXT
        //groupInfos.Add(MethodsGetter.GroupInfo);// TODO NEXT
        //groupInfos.Add(EventsGetter.GroupInfo);// TODO NEXT
    }
}




//public class PublicFieldsGetter : InspectorGroupGetter
//{
//    public override string Key => "reflection:Data/Fields/Public";
//}



//public class ReflectionInspectorGroupGetter : IEnumerableGetter<IObservableCache<INode, string>>
//{
//    ConcurrentDictionary<Type, IDictionary<string, >>
//    static ReflectionInspectorGroupGetter Get(object source)
//    {
//        var type = source.GetType();
//    }
//}


//public class ReflectionInspectionGroups
//{
//    public InspectorGroupGetter PropertyMembers
//    {
//    }
//}

//public class GrainInspector : IInspector
//{
//    public IEnumerableGetter<IObservableCache<INode, string>> GroupsForObject(object source)
//    {
//        throw new NotImplementedException();
//    }

//    public bool IsSourceSubscribable(object source)
//    {
//        throw new NotImplementedException();
//    }

//}

//public class FlexInspector : IInspector
//{
//}

//public class VobInspector : IInspector
//{    
//}
