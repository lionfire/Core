using DynamicData.Binding;
using LionFire.Data.Async;
using LionFire.Data.Async.Gets;
using LionFire.Data.Async.Sets;
using LionFire.FlexObjects;
using LionFire.Inspection.Nodes;
using LionFire.Mvvm;
using LionFire.Overlays;
using LionFire.Structures;
using ReactiveUI;
using System.Diagnostics;

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
    #endregion

    #endregion

    #region Node

    public INode Node { get; init; }
    INode? IReadWrapper<INode>.Value => Node;

    #endregion

    #endregion

    #region Lifecycle

    public NodeVM(INode node) : this(null, node)
    {
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

        #region Getter

        Getter = node.Source as IGetter<object>;

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

        if (node is IInspectedNode i && i.Groups.Count > 0)
        {
            ChildrenVM = new NodeChildrenVM(this);

            // Propagate AreChildrenVisible to ChildrenVM.OnExpand()
            this.WhenAnyValue(x => x.AreChildrenVisible)
                .Subscribe(areChildrenVisible =>
                {
                    if (areChildrenVisible)
                    {
                        ChildrenVM.OnExpand();
                    }
                });
        }

        #endregion
    }

    #endregion

    #region Options

    #region Options: this

#warning TODO REVIEW: Use Options on Node instead of NodeVM?

    /// <summary>
    /// Local options, not inherited.  See EffectiveOptions for inherited options.
    /// </summary>
    public InspectorOptions? Options { get; set; }
    public InspectorOptions GetLocalOptions() => Options ??= new();
    IInspectorOptions? IHas<IInspectorOptions>.Object => Options;

    #endregion

    #region Options: inherited

    /// <summary>
    /// </summary>
    /// <remarks>
    /// ENH: Inheritance on a per-property basis
    /// </remarks>
    public IInspectorOptions InheritedOptions
        => OverlayX_NEW.NextNonNull<IInspectorOptions, NodeVM>(this, InspectorOptions.Default) ?? InspectorOptions.DefaultDefault;

    //public IInspectorOptions EffectiveOptions => OverlayX.GetEffective<IInspectorOptions, NodeVM>(vm => (vm.Options, vm.Parent), InspectorOptions.Default);
    //public IInspectorOptions EffectiveOptions => /* lazily get Source Generated Overlay Proxy */ // ENH - ideal?

    #endregion

    [Reactive]
    public bool AreChildrenVisible { get; set; } // RENAME ShowChildren (or IsExpanded)

    #endregion

    #region Flex

    object? IFlex.FlexData { get; set; }

    #endregion

    #region Value

    #region Accessors

    public bool CanRead => Getter != null || AsyncValue != null;
    public bool CanWrite => SetValue != null;


    #region Read

    public IGetter<object>? Getter { get; private set; }

    #endregion

    #region ReadWrite

    public AsyncValue<object>? AsyncValue { get; private set; }

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

    public object? Value
    {
        get
        {
            var asyncValue = AsyncValue;
            if (asyncValue != null) { return asyncValue.ReadCacheValue; }
            var getter = Getter;
            if (getter != null) { return getter.ReadCacheValue; }
            if (asyncValue != null) { return asyncValue.StagedValue; }
            var setter = Setter;
            if (setter != null) { return setter.StagedValue; }
            return default;
        }
        set
        {
            var asyncValue = AsyncValue;
            if (asyncValue != null) { asyncValue.StagedValue = value; }
            var setter = Setter;
            if (setter != null) { setter.StagedValue = value; }
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
