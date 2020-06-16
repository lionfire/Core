using LionFire.Alerting;
using LionFire.Applications;
using LionFire.Shell;
using LionFire.Shell.Wpf;
using LionFire.Threading;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace LionFire.Hosting
{
    public static class WpfShellHostingExtensions
    {
        public static IServiceCollection AddWpfShell(this IServiceCollection services)
            => services
                .AddSingleton<IWpfShell>(p => p.GetRequiredService<WpfShell>()) // REVIEW - can these two lines be merged?
                .AddSingleton<WpfShell>()

                //.AddSingleton<IWindowManager, LionFireWindowManager>()

                .AddTransient<IShellPresenter, WpfShellPresenter>()
                //.AddSingleton<IShellPresenter>(s=>s.GetRequiredService<WpfShellPresenter>())
                //.AddSingleton<WpfShellPresenter>()

                .AddSingleton<IAlerter, WpfShellAlerter>()
                .Configure<AppOptions>(o => o.AddHostedService<WpfShell>())
                ;

        public static IServiceCollection SetWpfApplication(this IServiceCollection services, Application application)
            => services
                .AddSingleton(application ?? throw new ArgumentNullException(nameof(application)))
                ;

        public static IServiceCollection AddDispatcher(this IServiceCollection services, Application application)
            => services
                .AddSingleton<IDispatcher, WpfDispatcherAdapter>(serviceProvider => new WpfDispatcherAdapter(application))
                ;
    }
}
