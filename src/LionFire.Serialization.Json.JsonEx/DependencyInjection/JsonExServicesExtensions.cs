using LionFire.Serialization;
using LionFire.Serialization.Json.JsonEx;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Applications.Hosting;

namespace LionFire.Hosting;

public static class JsonExAppHostExtensions
{
    public static IServiceCollection AddJsonEx(this IServiceCollection services)
    {
        services
            .AddSingleton<JsonExLionFireSerializer>()
            .TryAddEnumerableSingleton<ISerializationStrategy, JsonExLionFireSerializer>()
            ;
        return services;
    }
}
