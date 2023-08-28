

using DynamicData.Binding;

namespace LionFire.Inspection.Nodes;

//public class GetterInspector : IInspector
//{
//    public IReadOnlyDictionary<string, InspectorGroupInfo> GroupInfos { get; } = new();

//    public GetterInspector()
//    {
//        GroupInfos.Add(new GetterInspectorGroupInfo());
//    }

//private void OnSourceTypeChanged(object source) // TODO
//{
//    if (source is IGetter g)
//    {
//        // ENH: Also support IStatelessGetter somehow

//        var getterType = g.GetGetterTypes().SingleOrDefault(); // TODO - throws if more than one Getter type

//        if (getterType != null)
//        {
//            var objGetter = (IGetter<object>)g;
//            objGetter.GetResults.Subscribe(getResult =>
//            {
//                node.Groups.Edit(updater =>
//                {
//                    foreach (var group in GroupsForObject(node, getResult.Value)
//                            .Where(g => !updater.Keys.Contains(g.Info.Key)))
//                    {
//                        updater.AddOrUpdate(group);
//                    }
//                });
//            });
//        }
//    }

//}
//}

//public class GetterInspectorGroup : InspectorGroup
//{
//    public GetterInspectorGroup(IInspector inspector, InspectorGroupInfo info, object source, INode node) : base(inspector, info, source, node)
//    {
//    }

//    public override IEnumerable<INode>? Value => throw new NotImplementedException();

//    public override IEnumerable<INode>? ReadCacheValue => throw new NotImplementedException();

//    public override void DiscardValue()
//    {
//        throw new NotImplementedException();
//    }

//    public override ITask<IGetResult<IEnumerable<INode>>> GetImpl(CancellationToken cancellationToken = default)
//    {
//        throw new NotImplementedException();
//    }
//}

//public class GetterInspectorGroupInfo : InspectorGroupInfo
//{
//    public GetterInspectorGroupInfo(IInspector inspector) : base("Getter")
//    {
//        Inspector = inspector;
//    }

//    public IInspector Inspector { get; }

//    public override InspectorGroup CreateFor(INode node)
//    {
//        return new GetterInspectorGroup();
//    }

//}

//public class GetterNode : Node
//{
//    public override SourceCache<InspectorGroup, string> Groups => throw new NotImplementedException();

//    public override IObservableList<INode> Children => throw new NotImplementedException();
//}