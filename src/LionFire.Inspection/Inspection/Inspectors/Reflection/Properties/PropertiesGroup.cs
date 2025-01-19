using LionFire.Data.Async.Gets;
using LionFire.Data.Collections;
using ReactiveUI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;

namespace LionFire.Inspection.Nodes;

public class PropertiesGroup : GroupNode, IConfiguredGetter, IInspectorGroup, IHas<IStatelessGetter<object>>
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

        Children = new PropertiesChildren(this);

        Task.Run(() =>
        {

            Parent.WhenAnyValue(x => x.Parent!.Value)
                .Subscribe(async v =>
                {
                    Children.DiscardValue();
                    await GetterOptions.TryAutoGet(this.Children).AsTask();//.Wait(); // Blocking, but it's synchronous
                });
        });
    }

    #endregion

    #region Children

    public override IAsyncReadOnlyKeyedCollection<string, INode> Children { get; }
    IStatelessGetter<object>? IHas<IStatelessGetter<object>>.Object => Children;

    private class PropertiesChildren : AsyncReadOnlyKeyedCollection<string, INode>

    {
        private PropertiesGroup propertiesGroup;

        public PropertiesChildren(PropertiesGroup propertiesGroup) : base(n => n.Key)
        {
            this.propertiesGroup = propertiesGroup;
        }

        private static readonly BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

        // REVIEW - can/should some of this be done once, statically, and/or synchronously?
        protected override ITask<IGetResult<IEnumerable<INode>>> GetImpl(CancellationToken cancellationToken = default)
        {
            var v = propertiesGroup.Value;
            return Task.FromResult<IGetResult<IEnumerable<INode>>>(
                GetResult<IEnumerable<INode>>.SyncSuccess(
                v == null
                  ? []
                  : v.GetType().GetProperties(bindingFlags)
                  .Where(mi => mi.GetIndexParameters().Length == 0)
                  //.Select(mi => new KeyValuePair<string, INode>(mi.Name, new PropertyNode(propertiesGroup, v, mi)))
                  .Select(mi => new PropertyNode(propertiesGroup, v, mi))
                  )).AsITask();
        }
    }

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