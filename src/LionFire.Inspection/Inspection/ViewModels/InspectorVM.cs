using LionFire.Mvvm;
using ReactiveUI;

namespace LionFire.Inspection.ViewModels;

public class InspectorVM : ReactiveObject
{
    #region Dependencies

    public InspectorService ObjectInspectorService { get; }

    #endregion

    #region Node

    #region NodeVM

    //[EditorRequired]
    [SetOnce]
    public NodeVM NodeVM
    {
        get => nodeVM;
        set
        {
            if (nodeVM == value) return;
            if (nodeVM != default) throw new AlreadySetException();
            nodeVM = value;
        }
    }
    private NodeVM nodeVM = null!;

    #endregion

    #endregion

    #region Derived

    //public TypeInteractionModel? TypeModel => TypeInteractionModels.Get(SourceObject?.GetType());

    #endregion

    #region Lifecycle

    public InspectorVM(IViewModelProvider viewModelProvider, IServiceProvider serviceProvider, InspectorService objectInspectorService)
    {
        ViewModelProvider = viewModelProvider;
        ServiceProvider = serviceProvider;
        ObjectInspectorService = objectInspectorService;

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

    #region Title

    public string Title { get; set; } = "Properties";
    public bool ShowTitle { get; set; } = true;

    #endregion
    
    #region Item Filters

    [Reactive]
    public bool ShowFilterTypes { get; set; }

    IInspectorOptions Options => NodeVM.InheritedOptions;
    InspectorOptions LocalOptions => NodeVM.GetLocalOptions();

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

    #region Items

    public IViewModelProvider ViewModelProvider { get; }
    public IServiceProvider ServiceProvider { get; }

    //public bool CanRead(ReflectionMemberVM member)
    //    => member.Info.ReadRelevance.HasFlag(ReadRelevance);

    //public bool CanWrite(ReflectionMemberVM member)
    //    => member.Info.WriteRelevance.HasFlag(WriteRelevance);

    #endregion

    public bool DevMode { get; set; } = true;
}
