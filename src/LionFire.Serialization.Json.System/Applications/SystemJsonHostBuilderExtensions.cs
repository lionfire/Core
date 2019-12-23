using LionFire.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using LionFire.Dependencies;
using Microsoft.Extensions.Hosting;
using LionFire.Serialization.Json.System;

namespace LionFire.Services
{
    public static class SystemJsonHostBuilderExtensions
    {
        public static IHostBuilder AddJson(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) => services.TryAddEnumerableSingleton<ISerializationStrategy, SystemJsonSerializer>());
            return hostBuilder;
        }
        public static IServiceCollection AddJson(this IServiceCollection app) => app.TryAddEnumerableSingleton<ISerializationStrategy, SystemJsonSerializer>();
    }
}
