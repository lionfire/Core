using LionFire.Data.Async;

namespace LionFire.Inspection;

public class InspectorContext
{

}

public class InspectorNode : INode
{
    public object Source { get; }


    #region Lifecycle

    public InspectorNode(object source)
    {
        Source = source;
    }

    #endregion

    IObservableCache<InspectorGroupGetter, string> INode.Groups => groups.AsObservableCache();

    public SourceCache<InspectorGroupGetter, string> WriteableGroups => groups;
    SourceCache<InspectorGroupGetter, string> groups = new SourceCache<InspectorGroupGetter, string>(x => x.Key);
}

//public abstract class InspectorNode<TInfo> : ReactiveObject, INode
//    where TInfo : INodeInfo
//{
//    public TInfo Info { get; }
//    IInspectorMemberInfo INode.Info => Info;

//    public InspectorContext Context { get; }

//    public IObservableCache<InspectionGroupGetter, string> Groups => throw new NotImplementedException();

//    public object Source => throw new NotImplementedException();

//    public InspectorNode(TInfo info)
//    {
//        Info = info;
//    }

//}

public abstract class MemberVM : InspectorNode<IInspectorMemberInfo>
{
    protected MemberVM(IInspectorMemberInfo info) : base(info)
    {
    }

    public abstract object Source { get; }
}
