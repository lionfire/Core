using LionFire.Data.Async.Gets;
using LionFire.Data.Collections;
using LionFire.Data.Mvvm;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace LionFire.Inspection.Nodes;


public class PropertiesGroup : SyncFrozenGroup
{
    public static readonly GroupInfo GroupInfo = new PropertiesGroupInfo();

    public static GetterOptions DefaultGetterOptions = new GetterOptions
    {
        AutoGet = true,
    };

    public INode SourceNode => Parent!;

    #region Parameters

    public GetterOptions GetterOptions => DefaultGetterOptions;

    #endregion

    #region Lifecycle

    public PropertiesGroup(IInspector inspector, INode parent, InspectorContext? context = null) : base(inspector, parent, GroupInfo, "group:Properties", context)
    {
        ArgumentNullException.ThrowIfNull(parent, nameof(parent));

        Parent.WhenAnyValue(x => x.SourceType)
            .Subscribe(t =>
            {
                DiscardValue();
                GetterOptions.TryAutoGet(this.Children).AsTask().Wait();
            });
    }

    #endregion

    #region Get

    protected override IEnumerable<KeyValuePair<string, INode>> GetChildren()
        => SourceType == null
              ? Enumerable.Empty<KeyValuePair<string, INode>>()  // Empty
              : SourceType.GetProperties(System.Reflection.BindingFlags.Instance).Select(mi =>
                  new KeyValuePair<string, INode>(mi.Name, new PropertyNode(this, SourceNode, mi))
                      );

    #endregion

    
}

public class PropertiesGroupInfo : GroupInfo
{
    public const string DefaultKey = "Data/Property";

    public PropertiesGroupInfo() : base(DefaultKey)
    {
    }

    public override IInspectorGroup CreateFor(INode node)
    {
        throw new NotImplementedException();
        //return new PropertyNodesGetter()
    }

    public override bool IsSourceTypeSupported(Type type)
    {
        return !type.IsPrimitive;
    }
}