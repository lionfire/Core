using LionFire.Inspection.Nodes;
using LionFire.Mvvm;
using ReactiveUI;
using System.Reactive.Linq;

namespace LionFire.Inspection.ViewModels;

public partial class InspectorVM : ReactiveObject
{
    #region Dependencies

    public IInspectorService InspectorService => InspectorContext.InspectorService;
    public IViewModelProvider ViewModelProvider => InspectorContext.ViewModelProvider;
    public IServiceProvider ServiceProvider => InspectorContext.ServiceProvider;

    #endregion

    #region Parameters

    [ReactiveUI.SourceGenerators.Reactive]
    private object? _source;

    private InspectorContext InspectorContext { get; set; } 

    #endregion

    #region Node

    #region NodeVM

    public NodeVM NodeVM => nodeVM.Value;
    private readonly ObservableAsPropertyHelper<NodeVM> nodeVM;

    #endregion

    #endregion

    #region Derived

    public InspectedNode InspectedNode => (InspectedNode)NodeVM.Node;

    //public TypeInteractionModel? TypeModel => TypeInteractionModels.Get(SourceObject?.GetType());

    #endregion

    #region Lifecycle

    public InspectorVM(IViewModelProvider viewModelProvider, IInspectorService inspectorService)
    {
        InspectorContext = new(inspectorService, viewModelProvider);

        nodeVM = this.WhenAnyValue(x => x.Source)
                     .Select(o =>
                     {
                         var nodeVM = new NodeVM(new InspectedNode(o, InspectorContext));
                         nodeVM.GetLocalOptions(true);
                         return nodeVM;
                     })
                     .ToProperty(this, nameof(this.NodeVM));
    }

    #endregion

    #region Options

    public bool DevMode { get; set; } = true;

    #region Item Filters

    [ReactiveUI.SourceGenerators.Reactive]
    private bool _showFilterTypes;

    IInspectorOptions Options => NodeVM.Options;
    InspectorOptions LocalOptions => NodeVM.GetLocalOptions();

    #endregion

    #region Items


    //public bool CanRead(ReflectionMemberVM member)
    //    => member.Info.ReadRelevance.HasFlag(ReadRelevance);

    //public bool CanWrite(ReflectionMemberVM member)
    //    => member.Info.WriteRelevance.HasFlag(WriteRelevance);

    #endregion
    
    #endregion

}

