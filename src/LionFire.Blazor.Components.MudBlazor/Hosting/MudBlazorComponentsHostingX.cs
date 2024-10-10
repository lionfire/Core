using LionFire.Blazor.Components.MudBlazor_;
using LionFire.Blazor.Components.MudBlazor_.PropertyGrid;
using LionFire.Inspection.ViewModels;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace LionFire.Hosting;

public static class MudBlazorComponentsHostingX
{
    public static IServiceCollection AddMudBlazorComponents(this IServiceCollection services)
    {
        return services
            .AddUIComponents()
            .AddSingleton<MudBlazorViewTypeProvider>()
            .AddSingleton<IViewTypeProvider, MudBlazorViewTypeProvider>(sp => sp.GetRequiredService<MudBlazorViewTypeProvider>())

            .AddTransient<InspectorValueCellVM>()
            .AddTransient(typeof(InspectorValueCellVM<>))
            
            ;
    }

    public static IServiceCollection AddPropertiesConfigurator<TTarget, TConfigurator>(this IServiceCollection services)
        where TConfigurator : class, IPropertiesConfigurator<TTarget>
    {
        return services
            .AddSingleton<IPropertiesConfigurator<TTarget>, TConfigurator>()
            ;
    }
}

//#error NEXT: make a default one of these, and use it on the View side
public interface IPropertiesConfigurator<T>
{
    float Order { get; set; }
    void Configure(IEnumerable<IPropertyConfig> propertyConfigs);

}

public interface IPropertyConfig
{
    PropertyInfo PropertyInfo { get; set; }
    bool Visible { get; set; }
}


