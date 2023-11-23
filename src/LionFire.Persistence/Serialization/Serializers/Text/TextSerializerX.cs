using LionFire.Hosting;
using LionFire.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

public static class TextSerializerX
{
    public static IServiceCollection AddTextSerializer(this IServiceCollection services)
    {
        services.TryAddEnumerableSingleton<ISerializationStrategy, TextSerializer>();
        services.AddSingleton<TextSerializer>();
        return services;
    }
}