using LionFire.Dependencies;
using LionFire.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TextSerializerExtensions
    {
        public static IServiceCollection AddTextSerializer(this IServiceCollection services)
        {
            services.TryAddEnumerableSingleton<ISerializationStrategy, TextSerializer>();
            services.AddSingleton<BinarySerializer>();
            return services;
        }
    }
}