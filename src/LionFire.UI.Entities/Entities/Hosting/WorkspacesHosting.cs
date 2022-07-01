using LionFire.Hosting;
using LionFire.UI.Workspaces;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting
{
    public static class WorkspacesHosting
    {
        public static LionFireHostBuilder Workspaces(this LionFireHostBuilder lf)
            => lf.ConfigureServices(services => services.AddWorkspaces());

        public static IServiceCollection AddWorkspaces(this IServiceCollection services)
            => services.AddSingleton<IWorkspaceProvider, InMemoryWorkspaceProvider>(); // TODO: let user specify another implementation

    }
}
