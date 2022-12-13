using LionFire.Dependencies;
using LionFire.Serialization;
using LionFire.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TextSerializerExtensions
    {
        public static IServiceCollection AddTextSerializer(this IServiceCollection services)
        {
            services.TryAddEnumerableSingleton<ISerializationStrategy, TextSerializer>();
            services.AddSingleton<TextSerializer>();
            return services;
        }
    }
}