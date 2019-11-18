using LionFire.Applications.Hosting;
using Microsoft.Extensions.Hosting;
using LionFire.Dependencies;
using System;

namespace LionFire.Applications.Hosting
{
    public static class AppHostExtensions
    {
        public static IAppHost UseDefaults(this IAppHost app)
        {
            _useDefaults();
            return app;
        }

        private static void _useDefaults()
        {
            //SingletonConfiguration.UseSingletons = true; // already default
            DependencyContext.Default.UseAsGuaranteedSingletonProvider();
            throw new NotSupportedException("Obsolete.  Use 'UseDependencyContext' instead");
        }

        public static IHostBuilder UseLionFireDefaults(this IHostBuilder hostBuilder)
        {
            _useDefaults();
            return hostBuilder;
        }

    }
}
