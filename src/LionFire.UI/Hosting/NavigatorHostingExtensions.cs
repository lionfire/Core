using LionFire.DependencyMachines;
using LionFire.Services.DependencyMachines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Hosting
{
    public static class NavigatorHostingExtensions
    {
        public static IServiceCollection AddNavigator<TNavigator>(this IServiceCollection services, Action<IParticipant> configure = null)
            where TNavigator : class, IHostedService
            => services
                .AddSingletonHostedServiceDependency<TNavigator>(p =>
                {
                    configure?.Invoke(p);
                    p.After(DependencyConventionsForUI.CanStartShell);
                });
    }
}
