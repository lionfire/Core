using LionFire.Serialization;
using LionFire.Serialization.Json.Newtonsoft;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using LionFire.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Applications.Hosting
{
    public static class NewtonsoftJsonAppHostExtensions
    {
        public static IAppHost AddNewtonsoftJson(this IAppHost app) => app.TryAddEnumerableSingleton<ISerializationStrategy, NewtonsoftJsonSerializer>();
        public static IHostBuilder AddNewtonsoftJson(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) => services.TryAddEnumerableSingleton<ISerializationStrategy, NewtonsoftJsonSerializer>());
            return hostBuilder;
        }
        public static IServiceCollection AddNewtonsoftJson(this IServiceCollection app) => app.TryAddEnumerableSingleton<ISerializationStrategy, NewtonsoftJsonSerializer>();
    }
}
