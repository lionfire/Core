using LionFire.Vos;
using Microsoft.Extensions.DependencyInjection;
using System;
using LionFire.ExtensionMethods;

namespace LionFire.UI;

public static class VosObjectExplorerRootsExtensions
{
    // TODO: Invoke this where you want to use ObjectExplorer
    // TODO: Make this an Action<IServiceProvider, ObjectExplorerRoots>
    public static IServiceCollection AddVosObjectExplorerRoot(this IServiceCollection services, IServiceProvider serviceProvider)
    {
        var vos = serviceProvider.GetService<IVos>();
        if (vos != null)
        {
            services.Configure<ObjectExplorerRoots>(o => o.Roots.Add("vos", vos));
        }
        return services;
    }
}