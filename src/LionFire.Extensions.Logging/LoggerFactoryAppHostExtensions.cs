using LionFire.Applications.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LionFire.Extensions.Logging
{
    public static class LoggerFactoryAppHostExtensions
    {
        public static IAppHost AddLogging(this IAppHost app, Action<ILoggerFactory> factoryInitializer)
        {
            var fac = new LoggerFactory();
            app.ConfigureServices(sc => sc.AddSingleton<ILoggerFactory>(fac));
            factoryInitializer(fac);
            return app;
        }
    }
}