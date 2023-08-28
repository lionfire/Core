using LionFire.FlexObjects;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Inspection;

public class InspectorContext : IFlex
{
    #region Relationships

    public IServiceProvider ServiceProvider { get => serviceProvider ?? InspectorService.ServiceProvider; set => serviceProvider = value; }
    IServiceProvider? serviceProvider;

    public IInspectorService InspectorService { get; }

    #endregion

    #region Lifecycle

    public InspectorContext(IInspectorService objectInspectorService)
    {
        InspectorService = objectInspectorService;
    }

    #endregion

    object? IFlex.FlexData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

}
