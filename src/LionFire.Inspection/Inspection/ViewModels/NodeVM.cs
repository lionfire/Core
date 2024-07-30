using DynamicData.Binding;
using LionFire.Data.Async;
using LionFire.Data.Async.Gets;
using LionFire.Data.Async.Sets;
using LionFire.ExtensionMethods.Cloning;
using LionFire.FlexObjects;
using LionFire.Inspection.Nodes;
using LionFire.IO;
using LionFire.Mvvm;
using LionFire.Overlays;
using LionFire.Structures;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive.Linq;

namespace LionFire.Inspection.ViewModels;

/// <summary>
/// A parallel hierarchy of ViewModels for INode.
///  - inheritable options. 
///  - IFlex support for attaching arbitrary data
/// </summary>
public class NodeVM : ReactiveObject, IViewModel<INode>, IParented<NodeVM>, IHas<IInspectorOptions>, IFlex
{
    #region Relationships

    #region Parent

    public NodeVM? Parent { get; }

    #region Derived

    public int Depth
    {
        get
        {
            int depth = 0;
            for (var parent = Parent; parent != null; parent = parent.Parent) { depth++; }
            return depth;
        }
    }

    /// <summary>
    /// Depth with flattened nodes not counted
    /// </summary>
    public int IndentLevel
    {
        get
        {
            int indents = 0;
            NodeVM prior = this;
            for (var parent = Parent; parent != null; prior = parent, parent = parent.Parent)
            {
                if (!prior.IsFlattened)
                {
                    indents++;
                }
            }
            return indents;
        }
    }
    #endregion

    #endregion

    public object Tag { get; set; } // TEMP

    #region Node

    public INode Node { get; init; }
    INode? IReadWrapper<INode>.Value => Node;

    #endregion

    #endregion

    #region Lifecycle

    public NodeVM(INode node) : this(null, node)
    {
        Tag = node.Path;
    }

    public void ApplyShowChildrenOptions()
    {
        if (Depth < Options.ShowChildrenForDepthBelow
            || (Node.Info.NodeKind & Options.ShowChildrenForNodeKinds) == Node.Info.NodeKind
            || IsFlattened)
        {
            ShowChildren = true;
        }
    }

    public bool IsFlattened
    {
        get
        {
            if ((Node.Info.NodeKind & Options.FlattenedNodeKinds) == Node.Info.NodeKind
                && !Options.ShowAll)
            {
                return true;
            }
            return false;
        }
    }

    public NodeVM(NodeVM parent, INode node)
    {
        ArgumentNullException.ThrowIfNull(node, nameof(node));

        Parent = parent;
        Node = node;

#if OLD
        // TODO - lang-ext discriminated unions for MemberVMs?  To handle multiple types while still providing type safety and API discoverability?
        //MemberVMs = ReflectionMemberVM.GetFor(InspectedObject?.EffectiveObject); // TypeModel?.Members.Select(m => MemberVM.Create(m, o)).ToList() ?? new();
#endif

        #region Async Value

        // REVIEW: don't fall back to Source?  Wire this up via Node implementation?
        AsyncValue = node as IAsyncValue<object> ?? (node as IHas<IAsyncValue<object>>)?.Object ?? node.Source as AsyncValue<object>;
        Getter = node as IGetter<object> ?? (node as IHas<IGetter<object>>)?.Object ?? node.Source as IGetter<object>;
        // ENH: Setter for completeness
        ValueState = AsyncValue as IValueState<object> ?? Getter as IValueState<object>;

        #endregion

        #region Setter properties: StagingSetTypes, NonstagingSetTypes, SetStagedValue, SetValue

        if (node.Source is ISetter setter)
        {
            StagingSetTypes = StagesSetWriter.GetStagesSetTypes(setter).ToArray();

            if (StagingSetTypes.Length == 0) { SetStagedValue = null; }
            else if (StagingSetTypes.Length > 1) throw new NotImplementedException("More than one IWriteStagesSet<> interface not implemented");
            else
            {
                var propertyInfo = typeof(IStagesSet<>).MakeGenericType(StagingSetTypes[0]).GetProperty(nameof(IStagesSet<object>.StagedValue))!;
                SetStagedValue = val => propertyInfo.SetValue(setter, val);
            }

            NonstagingSetTypes = StagesSetWriter.GetNonstagingSetterTypes(setter).ToArray();

            if (NonstagingSetTypes.Length == 0) { SetValue = null; }
            else if (NonstagingSetTypes.Length > 1) throw new NotImplementedException("More than one Non-staging ISetter<> interface not implemented");
            else
            {
                var methodInfo = typeof(ISetter<>).MakeGenericType(NonstagingSetTypes[0]).GetMethod(nameof(ISetter<object>.Set))!;
                SetValue = val => methodInfo.Invoke(setter, new object[] { val, CancellationToken.None });
            }
        }
        else
        {
            SetStagedValue = null;
        }
        #endregion

        #region ChildrenVM

        bool mightHaveChildren = false;
        if (node is IInspectedNode i && i.Groups.Count > 0)
        {
            mightHaveChildren = i.Groups.Count > 0;
        }
        else if (node is IHierarchicalNode h)
        {
            mightHaveChildren = true;
        }
        if (mightHaveChildren)
        {
            ChildrenVM = new NodeChildrenVM(this);

            // Propagate ShowChildren to ChildrenVM.OnExpand()
            this.WhenAnyValue(x => x.ShowChildren)
                .Subscribe(areChildrenVisible =>
                {
                    if (areChildrenVisible)
                    {
                        ChildrenVM.OnExpand();
                    }
                });
        }

        #endregion

        ApplyShowChildrenOptions();

    }

    #endregion

    #region Options

    #region Options: this

#warning TODO REVIEW: Use Options on Node instead of NodeVM?

    /// <summary>
    /// Local options, not inherited.  See EffectiveOptions for inherited options.
    /// </summary>
    [Reactive]
    public InspectorOptions? LocalOptions { get; set; }
    public InspectorOptions GetLocalOptions(bool useDefaults = false) => LocalOptions ??= (useDefaults ? (InspectorOptions)InspectorOptions.DefaultDefault.Clone() : new());
    IInspectorOptions? IHas<IInspectorOptions>.Object => LocalOptions;

    #endregion

    #region Options: inherited

    /// <summary>
    /// </summary>
    /// <remarks>
    /// ENH: Inheritance on a per-property basis
    /// </remarks>
    public IInspectorOptions Options
        => OverlayX_NEW.NextNonNull<IInspectorOptions, NodeVM>(this, InspectorOptions.Default) ?? InspectorOptions.DefaultDefault;

    //public IInspectorOptions EffectiveOptions => OverlayX.GetEffective<IInspectorOptions, NodeVM>(vm => (vm.Options, vm.Parent), InspectorOptions.Default);
    //public IInspectorOptions EffectiveOptions => /* lazily get Source Generated Overlay Proxy */ // ENH - ideal?

    #endregion

    [Reactive]
    public bool ShowChildren { get; set; }

    #region Individual Options

    // TODO: Move this to NodeVM, and inherit down the tree
    //public bool ShowDataMembers
    //{
    //    get => NodeVM?.ShowDataMembers ?? false;
    //    set { if (NodeVM != null) NodeVM.ShowDataMembers = value; }
    //}

    public bool ShowDataMembers
    {
        get => Options.VisibleItemTypes.HasFlag(InspectorNodeKind.Data);
        set
        {
            if (value) GetLocalOptions().VisibleItemTypes |= InspectorNodeKind.Data;
            else GetLocalOptions().VisibleItemTypes &= ~InspectorNodeKind.Data;
        }
    }
    public bool ShowEvents
    {
        get => Options.VisibleItemTypes.HasFlag(InspectorNodeKind.Event);
        set
        {
            if (value) GetLocalOptions().VisibleItemTypes |= InspectorNodeKind.Event;
            else GetLocalOptions().VisibleItemTypes &= ~InspectorNodeKind.Event;
        }
    }
    public bool ShowMethods
    {
        get => Options.VisibleItemTypes.HasFlag(InspectorNodeKind.Method);
        set
        {
            if (value) GetLocalOptions().VisibleItemTypes |= InspectorNodeKind.Method;
            else GetLocalOptions().VisibleItemTypes &= ~InspectorNodeKind.Method;
        }
    }

    public bool ShowHidden
    {
        get => Options.VisibilityFlags.HasFlag(InspectorVisibility.Hidden);
        set
        {
            if (value) GetLocalOptions().VisibilityFlags |= InspectorVisibility.Hidden;
            else GetLocalOptions().VisibilityFlags &= ~InspectorVisibility.Hidden;
        }
    }

    public bool ShowDev
    {
        get => Options.VisibilityFlags.HasFlag(InspectorVisibility.Dev);
        set
        {
            if (value) GetLocalOptions().VisibilityFlags |= InspectorVisibility.Dev;
            else GetLocalOptions().VisibilityFlags &= ~InspectorVisibility.Dev;
        }
    }

    #endregion

    #endregion

    #region Flex

    object? IFlex.FlexData { get; set; }

    #endregion

    #region Value

    #region Accessors
    
    public IODirection IODirection
    {
        get
        {
            if (CanRead)
            {
                if (CanWrite) return IODirection.ReadWrite;
                else return IODirection.Read;
            }
            else if (CanWrite) return IODirection.Write;
            else return LionFire.IO.IODirection.Unspecified;
        }
    }


    #region Read

    public IGetter<object>? Getter { get; private set; }

    #endregion

    #region ReadWrite

    public IAsyncValue<object>? AsyncValue { get; private set; }
    
    public IValueState<object>? ValueState { get; private set; }

    #region Derived: Convenience

    public bool CanRead => ValueState?.CanRead == true;
    public bool CanWrite => ValueState?.CanWrite == true;

    #endregion

    #endregion

    #region Write

    public Type[]? StagingSetTypes { get; private set; }
    private Action<object>? SetStagedValue { get; set; }
    public bool CanSetStagedValue => SetStagedValue != null;

    public Type[]? NonstagingSetTypes { get; private set; }
    private Action<object>? SetValue { get; set; }
    public bool CanSetValue => SetValue != null;

    public IStagesSet<object>? Setter { get; private set; }
    public ISetterRxO<object>? SetterRxO { get; private set; }

    #endregion

#endregion

#region Derived: Value

#if BadIdea
    public object? SyncValue
    {
        get
        {

            var asyncValue = AsyncValue;
            if (asyncValue != null)
            {
                if (asyncValue.HasStagedValue) return asyncValue.StagedValue;
                return asyncValue.GetIfNeeded().GetAwaiter().GetResult().Value;
            }

            var setter = Setter;
            if (setter != null && setter.HasStagedValue) { return setter.StagedValue; }

            var getter = Getter;
            if (getter != null) { getter.GetIfNeeded().GetAwaiter().GetResult(); return getter.ReadCacheValue; }

            return default;
        }
        set
        {
            var asyncValue = AsyncValue;
            if (asyncValue != null) { asyncValue.Value = value; }
            else
            {
                var setter = Setter;
                if (setter != null) { setter.StagedValue = value; }
            }
        }
    }
#endif

    public object? Value
    {
        get
        {
            var asyncValue = AsyncValue;
            if (asyncValue != null)
            {
                if (asyncValue.HasStagedValue) return asyncValue.StagedValue;
                if (asyncValue.HasValue) { return asyncValue.ReadCacheValue; }
            }
            var setter = Setter;
            if (setter != null && setter.HasStagedValue) { return setter.StagedValue; }

            var getter = Getter;
            if (getter != null) { return getter.ReadCacheValue; }

            return default;
        }
        set
        {
            var asyncValue = AsyncValue;
            if (asyncValue != null) { asyncValue.Value = value; }
            else
            {
                var setter = Setter;
                if (setter != null) { setter.StagedValue = value; }
            }
        }
    }

    public Type? CurrentValueType { get; set; } // TODO - is this wired up?
    //public Type? Type => NodeVM?.Node?.Info.Type;

    public bool ValueTypeDiffersFromMemberType => CurrentValueType != null && CurrentValueType != Node.Info.Type; // REVIEW Type

    #region Display

    public string? DisplayValue { get; set; }
    public string ValueClass { get; set; } = ""; // TODO: ValueClasses enumerable

    #endregion

#endregion

#endregion

    #region Children

    public NodeChildrenVM? ChildrenVM { get; set; }
    public bool CanHaveChildren => ChildrenVM != null;
    public bool HasOrMightHaveChildren => ChildrenVM != null && ChildrenVM.HasChildren != false;

    #endregion

    #region TODO

    //public bool ReadOnly => InspectorVM?.ReadOnly == true;

    #endregion
}
