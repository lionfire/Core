using LionFire.Hosting;
using LionFire.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

public static class BinarySerializerX
{
    public static IServiceCollection AddBinarySerializer(this IServiceCollection services)
    {
        services.TryAddEnumerableSingleton<ISerializationStrategy, BinarySerializer>();
        services.AddSingleton<BinarySerializer>();
        return services;
    }
}