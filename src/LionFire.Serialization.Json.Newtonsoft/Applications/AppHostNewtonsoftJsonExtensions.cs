using LionFire.Serialization;
using LionFire.Serialization.Json.Newtonsoft;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using LionFire.Dependencies;
using Microsoft.Extensions.Hosting;
using LionFire.Applications.Hosting;

namespace LionFire.Services
{
    public static class NewtonsoftJsonAppHostExtensions
    {
        public static IAppHost AddNewtonsoftJson(this IAppHost app) => app.TryAddEnumerableSingleton<ISerializationStrategy, NewtonsoftJsonSerializer>();

        public static IHostBuilder AddNewtonsoftJson(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) => services.AddNewtonsoftJson());
            return hostBuilder;
        }

        public static IServiceCollection AddNewtonsoftJson(this IServiceCollection services)
        {
            services.TryAddEnumerableSingleton<ISerializationStrategy, NewtonsoftJsonSerializer>();
            services.AddSingleton<NewtonsoftJsonSerializer>();
            return services;
        }
    }
}
