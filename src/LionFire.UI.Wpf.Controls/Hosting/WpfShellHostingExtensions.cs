using LionFire.Alerting;
using LionFire.Applications;
using LionFire.Services.DependencyMachines;
using LionFire.Shell;
using LionFire.Shell.Wpf;
using LionFire.Threading;
using LionFire.UI.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using LionFire.DependencyMachines;
using LionFire.Settings;
using LionFire.UI.Windowing;
using LionFire.Vos;
using LionFire.Vos.VosApp;
using Microsoft.Extensions.Hosting;
using LionFire.Resolves;

namespace LionFire.Hosting
{
    public static class WpfShellHostingExtensions
    {
        /// <summary>
        /// Wire up minimal relationships for WPF to run:
        ///  - Application
        ///  - Dispatcher
        /// </summary>
        /// <param name="services"></param>
        /// <param name="application"></param>
        /// <returns></returns>
        public static IServiceCollection AddWpfRuntime(this IServiceCollection services, Application application)
          => services
              .SetWpfApplication(application)
              .AddDispatcher(application)
              .AddEventAggregator() // Uses IDispatcher
              ;

        public static IServiceCollection SetWpfApplication(this IServiceCollection services, Application application)
            => services
                .AddSingleton(application ?? throw new ArgumentNullException(nameof(application)))
                ;

        public static IServiceCollection AddDispatcher(this IServiceCollection services, Application application)
            => services
                .AddSingleton<WpfDispatcherAdapter>(serviceProvider => new WpfDispatcherAdapter(application))
                .AddSingleton<IDispatcher, WpfDispatcherAdapter>(serviceProvider => serviceProvider.GetRequiredService<WpfDispatcherAdapter>());

        public static IServiceCollection AddWpfAlerter(this IServiceCollection services, Application application)
            => services
                .AddSingleton<WpfAlerter>();

        public static IServiceCollection AddWpfShell(this IServiceCollection services)
        {
            WpfCommandManager.Initialize();

            return services

            #region Settings

                    // REVIEW - registering both IUserLocalSettings<WindowSettings> and IUserLocalSettings<>?  Maybe just register as UserLocalSettingsProvider<WindowSettings>

                    .AddSingletonHostedServiceDependency<ILazilyResolves<WindowSettings>, UserLocalSettingsProvider<WindowSettings>>(p => p.Contributes(DependencyConventionsForUI.CanStartShell))
                    //.AddSingletonHostedServiceDependency<IUserLocalSettings<WindowSettings>, UserLocalSettingsProvider<WindowSettings>>(p => p.Contributes(DependencyConventionsForUI.CanStartShell))

                    .Configure<SettingsOptions>(o =>
                    {
                        o.Handles.Add(VosAppSettings.UserLocal<WindowSettings>.H);
                    })

            #endregion

                   .AddSingleton<DesktopProfileManager>()
                   .AddSingletonHostedServiceDependency<WpfDesktopProfileDetector>(p => p.DependsOn("vos:$Settings"))

                   .AddSingletonHostedServiceDependency<IShellConductor, ShellConductor>(p => p.After(DependencyConventionsForUI.CanStartShell))

                   .Configure<ShellPresenterOptions>(o =>
                   {
                       // ENH: Replace with TTabbedWindowPresenter template with options (such as TabsVisibility, TabsLocation, etc.)
                       o.MainWindowPresenterType = typeof(TabbedWindowPresenter);
                       o.AuxiliaryWindowPresenterType = typeof(TabbedWindowPresenter);
                   })

                   .AddTransient<IPresenter, WpfPresenter>()

                  //.AddNavigator<WpfNavigator>(p =>
                  //    p.After(HostedServiceParticipant<WpfDesktopProfileDetector>.KeyForHostedServiceType)
                  //     .After("WPF.Services")
                  //     .DependsOn("vos:/")
                  //   //.DependsOn("vos:$UserLocalSettings")
                  //   )

                  //.AddSingleton<IWindowManager, LionFireWindowManager>()

                  //.AddSingleton<IShellPresenter>(s=>s.GetRequiredService<WpfShellPresenter>())
                  //.AddSingleton<WpfShellPresenter>()

                  .AddSingleton<IAlerter, WpfShellAlerter>()
                  //.Configure<AppOptions>(o => o.SetHostedServicesIfEmpty(typeof(WpfShell)))
                  ;
        }

    }
}
