using LionFire.Inspection.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

public static class UIComponentsHostingX
{
    public static IServiceCollection AddUIComponents(this IServiceCollection services)
    {
        return services
            .AddTransient<InspectorVM>()
            .AddTransient<InspectorRowVM>()
            .AddTransient<NodeChildrenVM>()            
            ;
    }
}
