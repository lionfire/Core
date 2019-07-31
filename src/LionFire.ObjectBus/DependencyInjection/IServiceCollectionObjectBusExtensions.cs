using LionFire.Applications;
using LionFire.ObjectBus;
using LionFire.Structures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.ObjectBus
{
    public static class IServiceCollectionObjectBusExtensions
    {
        public static IServiceCollection AddObjectBus<T>(this IServiceCollection sc)
            where T : class, IOBus, new()
        {
            var obus = SingletonConfiguration.UseSingletons ? ManualSingleton<T>.GuaranteedInstance : new T();

            obus.AddServices(sc);

            return sc;
        }
        public static IHostBuilder AddObjectBus<T>(this IHostBuilder host) 
            where T : class, IOBus, new()
        {
            host.ConfigureServices((context, services) => services.AddObjectBus<T>());
            return host;
        }
    }
}
