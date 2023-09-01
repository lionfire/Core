﻿using LionFire.Data.Async.Gets;
using ReactiveUI;

namespace LionFire.Inspection.Nodes;

public class PropertiesGroup : ReflectionGroup
{
    public static readonly GroupInfo GroupInfo = new PropertyGroupInfo();

    public override GroupInfo Info => GroupInfo;


    #region Lifecycle

    public PropertiesGroup(IInspector inspector, INode sourceNode, INode node) : base(inspector, sourceNode, node)
    {
        this.WhenAnyValue(x => x.SourceType)
            .Subscribe(t =>
            {
                DiscardValue();
                GetOptions?.TryAutoGet(this);
            });
    }

    #endregion



    public override IDictionary<string, INode>? Value => throw new NotImplementedException();

    #region Get

    public override ITask<IGetResult<IEnumerable<KeyValuePair<string, INode>>>> GetImpl(CancellationToken cancellationToken = default)
        => SourceType == null
            ? Task.FromResult<IGetResult<IEnumerable<KeyValuePair<string, INode>>>>(new GetResult<IEnumerable<KeyValuePair<string, INode>>>(Enumerable.Empty<KeyValuePair<string, INode>>(), true)).AsITask()
            : Task.FromResult<IGetResult<IEnumerable<KeyValuePair<string, INode>>>>(new GetResult<IEnumerable<KeyValuePair<string, INode>>>(SourceType.GetProperties(System.Reflection.BindingFlags.Instance).Select(mi => new KeyValuePair<string, INode>(mi.Name, new PropertyNode(Node, SourceNode, mi))))).AsITask();

    #endregion


}

public class PropertyGroupInfo : GroupInfo
{
    public const string DefaultKey = "Data/Property";

    public PropertyGroupInfo() : base(DefaultKey)
    {
    }

    public override InspectorGroup CreateFor(INode node)
    {
        throw new NotImplementedException();
        //return new PropertyNodesGetter()
    }

    public override bool IsSourceTypeSupported(Type type)
    {
        return !type.IsPrimitive;
    }
}