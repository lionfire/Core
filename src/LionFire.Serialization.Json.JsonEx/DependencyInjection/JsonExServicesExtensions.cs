using LionFire.Serialization;
using LionFire.Serialization.Json.JsonEx;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Applications.Hosting;

namespace LionFire.Services
{
    public static class JsonExAppHostExtensions
    {
        public static IServiceCollection AddJsonEx(this IServiceCollection services)
        {
            services.TryAddEnumerableSingleton<ISerializationStrategy, JsonExLionFireSerializer>();
            services.AddSingleton<JsonExLionFireSerializer>();
            return services;
        }
    }
}
