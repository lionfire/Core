using LionFire.Applications;
using LionFire.Shell;
using LionFire.Threading;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace LionFire.Hosting
{
    public static class WpfShellHostingExtensions
    {
        public static IServiceCollection AddWpfShell(IServiceCollection services)
            => services
                .AddSingleton<ILionFireShell>(p => p.GetRequiredService<WpfShell>()) // REVIEW - can these two lines be merged?
                .AddSingleton<WpfShell>()
                .Configure<AppOptions>(o => o.AddHostedService<WpfShell>())
                ;

        public static IServiceCollection SetWpfApplication(IServiceCollection services, Application application)
            => services
                .AddSingleton(application ?? throw new ArgumentNullException(nameof(application)))
                ;

        public static IServiceCollection AddDispatcher(IServiceCollection services, Application application)
            => services
                .AddSingleton<IDispatcher, WpfDispatcherAdapter>(serviceProvider => new WpfDispatcherAdapter(application))
                ;
    }
}
