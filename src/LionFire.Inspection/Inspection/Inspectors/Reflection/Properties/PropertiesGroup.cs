using LionFire.Data.Async.Gets;
using LionFire.Data.Collections;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace LionFire.Inspection.Nodes;

public class PropertiesGroup : GroupNode, IConfiguredGetter, IInspectorGroup
{
    public static readonly GroupInfo GroupInfo = new PropertiesGroupInfo();

    public static GetterOptions DefaultGetterOptions = new GetterOptions
    {
        AutoGet = true,
    };

    public INode SourceNode => Parent!;
    public override object? Value { get => Parent?.Value; set => throw new NotSupportedException(); }

    #region Parameters

    public GetterOptions GetterOptions => DefaultGetterOptions;

    #endregion

    #region Lifecycle

    public PropertiesGroup(IInspector inspector, INode parent, InspectorContext? context = null) : base(inspector, parent, GroupInfo, "group:Properties", context)
    {
        ArgumentNullException.ThrowIfNull(inspector, nameof(inspector));
        ArgumentNullException.ThrowIfNull(parent, nameof(parent));
        context ??= parent.Context;

        Parent.WhenAnyValue(x => x.Parent!.Value)
            .Subscribe(async v =>
            {
                Children.DiscardValue();
                await GetterOptions.TryAutoGet(this.Children).AsTask();//.Wait(); // Blocking, but it's synchronous
            });

        Children = new PropertiesChildren(this);
    }

    public class PropertiesChildren : AsyncReadOnlyDictionary<string, INode>
    {
        private PropertiesGroup propertiesGroup;

        public PropertiesChildren(PropertiesGroup propertiesGroup)
        {
            this.propertiesGroup = propertiesGroup;
        }

        protected override ITask<IGetResult<IEnumerable<KeyValuePair<string, INode>>>> GetImpl(CancellationToken cancellationToken = default)
        {
            var v = propertiesGroup.Value;
            return Task.FromResult<IGetResult<IEnumerable<KeyValuePair<string, INode>>>>(
                new GetResult<IEnumerable<KeyValuePair<string, INode>>>(
                v == null
                  ? Enumerable.Empty<KeyValuePair<string, INode>>()  // Empty
                  : v.GetType().GetProperties(System.Reflection.BindingFlags.Instance).Select(mi =>
                      new KeyValuePair<string, INode>(mi.Name, new PropertyNode(propertiesGroup, v, mi))
                          )
                  )
                  ).AsITask();
        }
    }

    public override IAsyncReadOnlyDictionary<string, INode> Children { get; }

    #endregion

    #region Get

    //protected override IEnumerable<KeyValuePair<string, INode>> GetChildren()
    //{
    //    var v = Value;
    //    return v == null
    //          ? Enumerable.Empty<KeyValuePair<string, INode>>()  // Empty
    //          : v.GetType().GetProperties(System.Reflection.BindingFlags.Instance).Select(mi =>
    //              new KeyValuePair<string, INode>(mi.Name, new PropertyNode(this, SourceNode, mi))
    //                  );
    //    //=> (SourceType == null || SourceType == InspectorConstants.NullType)
    //    //      ? Enumerable.Empty<KeyValuePair<string, INode>>()  // Empty
    //    //      : SourceType.GetProperties(System.Reflection.BindingFlags.Instance).Select(mi =>
    //    //          new KeyValuePair<string, INode>(mi.Name, new PropertyNode(this, SourceNode, mi))
    //    //              );
    //}
    #endregion
}

public class PropertiesGroupInfo : GroupInfo
{
    public const string DefaultKey = "Data/Property";

    public PropertiesGroupInfo() : base(DefaultKey)
    {
    }



    public override IInspectorGroup CreateNode(INode node, IInspector? inspector = null)
    {
        return new PropertiesGroup(inspector ?? Inspector, node);
    }

    public override bool IsTypeSupported(Type type)
    {
        return !type.IsPrimitive;
    }
}