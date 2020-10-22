using LionFire.Applications.ErrorReporting;
using LionFire.ErrorReporting;
using LionFire.Logging;
using LionFire.Services.DependencyMachines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using LionFire.DependencyMachines;

namespace LionFire.Hosting
{
    public static class ErrorReportingHostingExtensions
    {
        public static IServiceCollection AddErrorReporting<T>(this IServiceCollection services)
            where T : class, IHostedService, IErrorReporter
            => services.AddHostedServiceDependency<T>(p => p.After<IAppStartLogger>());
    }
}
