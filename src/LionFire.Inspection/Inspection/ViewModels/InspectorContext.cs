using LionFire.FlexObjects;
using LionFire.Inspection.ViewModels;
using LionFire.Mvvm;
using LionFire.Overlays;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Inspection;

public class InspectorContext : IFlex
{
    #region Relationships

    public IServiceProvider ServiceProvider { get => serviceProvider ?? InspectorService.ServiceProvider; set => serviceProvider = value; }
    IServiceProvider? serviceProvider;

    public IInspectorService InspectorService { get; }
    public IViewModelProvider? ViewModelProvider { get; }

    #endregion

    #region Lifecycle

    public InspectorContext(IInspectorService service, IViewModelProvider? viewModelProvider = null)
    {
        InspectorService = service;
        ViewModelProvider = viewModelProvider ?? service.ServiceProvider.GetService<IViewModelProvider>();
    }

    #endregion

    object? IFlex.FlexData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IInspectorOptions Options { get; set; }





}
