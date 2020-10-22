using LionFire.Shell;
using LionFire.Shell.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Hosting
{
    public static class ShellHostingExtensions
    {
        public static IServiceCollection AddShell(this IServiceCollection services)
        {
            return services
                .AddSingleton<IShellConductor, ShellConductor>()
                .AddAsHostedService<StopApplicationOnShellClosed>()
                ;
        }
    }
}
