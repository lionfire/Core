using LionFire.Inspection.Nodes;
using LionFire.Mvvm;
using ReactiveUI;
using System.Reactive.Linq;

namespace LionFire.Inspection.ViewModels;

public class InspectorVM : ReactiveObject
{
    #region Dependencies

    public InspectorService InspectorService { get; }
    public IViewModelProvider ViewModelProvider { get; }
    public IServiceProvider ServiceProvider { get; }

    #endregion

    #region Parameters

    [Reactive]
    public object Source { get; set; }

    private InspectorContext InspectorContext { get; set; } 

    #endregion

    #region Node

    #region NodeVM

    //[EditorRequired]
    //[Mutable]
    //public NodeVM NodeVM
    //{
    //    get => nodeVM;
    //    protected set
    //    {
    //        if (nodeVM == value) return;
    //        if (nodeVM != default) throw new AlreadySetException();
    //        nodeVM = value;
    //    }
    //}
    //private NodeVM nodeVM = null!;

    public NodeVM NodeVM => nodeVM.Value;
    private readonly ObservableAsPropertyHelper<NodeVM> nodeVM;

    #endregion

    #endregion

    #region Derived

    public InspectedNode InspectedNode => (InspectedNode)NodeVM.Node;

    //public TypeInteractionModel? TypeModel => TypeInteractionModels.Get(SourceObject?.GetType());

    #endregion

    #region Lifecycle

    public InspectorVM(IViewModelProvider viewModelProvider, IServiceProvider serviceProvider, InspectorService inspectorService)
    {
        ViewModelProvider = viewModelProvider;
        ServiceProvider = serviceProvider;
        InspectorService = inspectorService;
        InspectorContext = new(InspectorService);

        nodeVM = this.WhenAnyValue(x => x.Source)
                     .Select(o => new NodeVM( new InspectedNode(o, InspectorContext)))
                     .ToProperty(this, nameof(this.NodeVM));

        //this.WhenAnyValue<InspectorVM, object>(x => x!.NodeVM!.Node!.Source!)
        //    .Subscribe(o =>
        //    {
        //        if (o is NodeVM x) NodeVM = x;
        //        else
        //        {
        //            NodeVM = o == null ? null : ActivatorUtilities.CreateInstance<NodeVM>(ServiceProvider, o);
        //        }
        //        Title = o?.GetType().Name.ToDisplayString() ?? "???";
        //    });
    }

    #endregion

    //#region Title  // OBSOLETE - move to something like WidgetWrapperVM

    //public string Title { get; set; } = "Properties";
    //public bool ShowTitle { get; set; } = true;

    //#endregion

    #region Options

    public bool DevMode { get; set; } = true;

    #region Item Filters

    [Reactive]
    public bool ShowFilterTypes { get; set; }

    IInspectorOptions Options => NodeVM.InheritedOptions;
    InspectorOptions LocalOptions => NodeVM.GetLocalOptions();

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
            if (value) LocalOptions.VisibleItemTypes |= InspectorNodeKind.Data;
            else LocalOptions.VisibleItemTypes &= ~InspectorNodeKind.Data;
        }
    }
    public bool ShowEvents
    {
        get => Options.VisibleItemTypes.HasFlag(InspectorNodeKind.Event);
        set
        {
            if (value) LocalOptions.VisibleItemTypes |= InspectorNodeKind.Event;
            else LocalOptions.VisibleItemTypes &= ~InspectorNodeKind.Event;
        }
    }
    public bool ShowMethods
    {
        get => Options.VisibleItemTypes.HasFlag(InspectorNodeKind.Method);
        set
        {
            if (value) LocalOptions.VisibleItemTypes |= InspectorNodeKind.Method;
            else LocalOptions.VisibleItemTypes &= ~InspectorNodeKind.Method;
        }
    }

    #endregion

    #endregion

    #region Items


    //public bool CanRead(ReflectionMemberVM member)
    //    => member.Info.ReadRelevance.HasFlag(ReadRelevance);

    //public bool CanWrite(ReflectionMemberVM member)
    //    => member.Info.WriteRelevance.HasFlag(WriteRelevance);

    #endregion
    #endregion

}
