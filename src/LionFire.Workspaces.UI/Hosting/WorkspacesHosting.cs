using LionFire.Hosting;
using LionFire.Reactive.Persistence;
using LionFire.UI.Workspaces;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

public static class WorkspacesHosting
{
    //public static ILionFireHostBuilder Workspaces(this ILionFireHostBuilder lf)
    //    => lf.ConfigureServices(services => services.AddWorkspaces());

    public static IServiceCollection AddWorkspacesUI(this IServiceCollection services)
        => services
        .AddTransient<WorkspaceGridVM>()
        ;

}
