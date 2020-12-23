using LionFire.Alerting;
using LionFire.Applications;
using LionFire.Hosting;
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
using LionFire.UI;
using LionFire.UI.Wpf;
using LionFire.UI.Entities;
using LionFire.UI.Entities.Wpf;

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
        public static IServiceCollection AddWpfRuntime(this IServiceCollection services, Application application = null)
          => services
              .SetWpfApplication(application ?? Application.Current ?? new Application())
              .AddSingleton<IUIPlatform, WpfRuntime>()
              .AddWpfDispatcher(application)
              //.AddHostedService<WpfDiagnostics>(_ => new WpfDiagnostics { DataBindingSourceLevel = System.Diagnostics.SourceLevels.All })  // OPTIONAL
              .AddEventAggregator() // Uses IDispatcher
              ;

        #region (Private)

        private static IServiceCollection SetWpfApplication(this IServiceCollection services, Application application)
            => services
                .AddSingleton(application ?? throw new ArgumentNullException(nameof(application)))
                .AddSingleton(application.Dispatcher)
                ;

        private static IServiceCollection AddWpfDispatcher(this IServiceCollection services, Application application)
            => services
                //.AddSingleton<WpfDispatcherAdapter>(serviceProvider => new WpfDispatcherAdapter(application))
                .AddSingleton<WpfDispatcherAdapter>()
                .AddSingleton<IDispatcher, WpfDispatcherAdapter>(serviceProvider => serviceProvider.GetRequiredService<WpfDispatcherAdapter>());

        #endregion


        public static IServiceCollection AddWpfControls(this IServiceCollection services)
        {
            WpfCommandManager.Initialize();
            return services
                ;
        }

        public static IServiceCollection AddWpfWindowing(this IServiceCollection services)
        {
            #region Settings

            return services
                // REVIEW - registering both IUserLocalSettings<WindowSettings> and IUserLocalSettings<>?  Maybe just register as UserLocalSettingsProvider<WindowSettings>
                .AddSingletonHostedServiceDependency<ILazilyResolves<WindowSettings>, UserLocalSettingsProvider<WindowSettings>>(p 
                => p
                    .Contributes(DependencyConventionsForUI.CanStartShell)
                    .After("vos:/") // TODO - what should this be?
                )

                //.AddSingletonHostedServiceDependency<IUserLocalSettings<WindowSettings>, UserLocalSettingsProvider<WindowSettings>>(p => p.Contributes(DependencyConventionsForUI.CanStartShell))

                .Configure<SettingsOptions>(o =>
                {
                    o.Handles.Add(VosAppSettings.UserLocal<WindowSettings>.H);
                })

            #endregion

                .AddSingleton<DesktopProfileManager>()
                .AddSingletonHostedServiceDependency<WpfDesktopProfileDetector>(p => p.DependsOn("vos:$Settings"));
        }

        // OLD
        //public static IServiceCollection AddWpfShell(this IServiceCollection services)
        //{
        //    return services

        //        //.Configure<ShellPresenterOptions>(o =>
        //        //{
        //        //    // ENH: Replace with TTabbedWindowPresenter template with options (such as TabsVisibility, TabsLocation, etc.)
        //        //    o.MainWindowPresenterType = typeof(TabbedWindowPresenter);
        //        //    o.AuxiliaryWindowPresenterType = typeof();
        //        //})

        //        //.AddTransient<IPresenter, WpfPresenter>()

        //        //.AddNavigator<WpfNavigator>(p =>
        //        //    p.After(HostedServiceParticipant<WpfDesktopProfileDetector>.KeyForHostedServiceType)
        //        //     .After("WPF.Services")
        //        //     .DependsOn("vos:/")
        //        //   //.DependsOn("vos:$UserLocalSettings")
        //        //   )

        //        //.AddSingleton<IWindowManager, LionFireWindowManager>()

        //        //.AddSingleton<IShellPresenter>(s=>s.GetRequiredService<WpfShellPresenter>())
        //        //.AddSingleton<WpfShellPresenter>()

        //        //.AddSingleton<IAlerter, WpfShellAlerter>()
        //        //.Configure<AppOptions>(o => o.SetHostedServicesIfEmpty(typeof(WpfShell)))
        //        ;
        //}



        public static IServiceCollection AddUIEntitiesForWpf(this IServiceCollection services, Action<IParticipant> configure = null)
        {
            return services
                .AddUIEntities(configure)
                .AddSingleton<IWindowFactory, WpfWindowFactory>()
                .AddTransient<IWindow, WpfMultiplexedWindow>()
                .AddTransient<ILayers, WpfLayers>()
                .AddTransient<ITabs, WpfTabs>()
                .AddSingleton<IAlerter, WpfAlerter>()
                ;
        }
    }
}
