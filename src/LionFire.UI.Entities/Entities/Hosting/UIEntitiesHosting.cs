//using LionFire.Shell.Wpf;
using LionFire.DependencyMachines;
using LionFire.Hosting;
using LionFire.UI;
using LionFire.UI.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LionFire.Hosting
{
    public static class UIEntitiesHosting
    {
        //public static IServiceCollection AddWindowedUI(this IServiceCollection services)
        //{
        //    // TODO
        //    return services
        //        //.Configure<UIEntitiesOptions>(o=>o.RootType = typeof(WindowCollection))
        //        //.AddSingleton<IUIRoot, WindowCollection>()
        //        //.AddAsHostedService<StopApplicationOnShellClosed>()
        //        ;
        //}

        private static Action<IParticipant> defaultUIEntitiesServiceConfigurer = 
            p => p
            //.After("vos:")
            .After(DependencyConventionsForUI.CanStartShell)
            ;

        /// <summary>
        /// Registers UIEntitiesService with global DependencyMachine.
        /// 
        /// To configure startup interface: services.Configure<UIStartupOptions>(...)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static LionFireHostBuilder UIEntities(this LionFireHostBuilder lf, Action<IParticipant> configure = null)
            => lf.ForHostBuilder(builder => builder.ConfigureServices((context, services) => services
                .AddSingleton<UIStarter>()
                .AddSingleton<IUIFactory, UIFactory>()
                .AddSingleton<IUIRoot, UIRoot>()
                .AddSingleton<IUIFactory, UIFactory>()
                .AddSingletonHostedServiceDependency<UIEntitiesService>(c =>
                {
                    defaultUIEntitiesServiceConfigurer(c);
                    configure?.Invoke(c);
                })));
    }
}
