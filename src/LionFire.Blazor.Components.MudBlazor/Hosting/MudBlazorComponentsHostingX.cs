using LionFire.Blazor.Components.MudBlazor_;
using LionFire.Blazor.Components.MudBlazor_.PropertyGrid;
using LionFire.Inspection.ViewModels;

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
}
