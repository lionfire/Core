using LionFire.Applications.Hosting;
using Microsoft.Extensions.Hosting;
using LionFire.DependencyInjection;

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
        }

        public static IHostBuilder UseDefaults(this IHostBuilder hostBuilder)
        {
            _useDefaults();
            return hostBuilder;
        }

    }
}
