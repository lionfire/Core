using LionFire.Hosting;
using LionFire.UI.Workspaces;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting
{
    public static class WorkspacesHosting
    {
        public static LionFireHostBuilder Workspaces(this LionFireHostBuilder lf)
            => lf.ForHostBuilder(builder => builder.ConfigureServices((context, services) => services
                .AddSingleton<IWorkspaceProvider, InMemoryWorkspaceProvider>())); // TODO: let user specify another implementation

    }
}
