using LionFire.Applications.Hosting;
using LionFire.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LionFire.Hosting
{
    public static class SerializationHostingX
    {
        public static IServiceCollection AddSerialization(this IServiceCollection services, bool useDefaultSettings = true)
        {
            services
                .AddSingleton<ISerializationProvider, SerializationProvider>()
                .AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptionsMonitor<SerializationOptions>>().CurrentValue)
                ;

            //app.AddSingleton<ISerializationProvider, SerializationProvider>(serviceProvider =>
            //{
            //    var result = new SerializationProvider();
            //    if (useDefaultSettings)
            //    {
            //        result.DeserializationScorers = 
            //    }
            //});

            if (useDefaultSettings)
            {
                foreach (var type in DefaultScorers.DefaultDeserializerScorers)
                {
                    services.TryAddEnumerable(new ServiceDescriptor(typeof(IDeserializeScorer), type, ServiceLifetime.Singleton));
                }
                foreach (var type in DefaultScorers.DefaultSerializerScorers)
                {
                    services.TryAddEnumerable(new ServiceDescriptor(typeof(ISerializeScorer), type, ServiceLifetime.Singleton));
                }
            }
            return services;
        }

        public static IHostBuilder AddSerialization(this IHostBuilder hostBuilder, bool useDefaultSettings = true)
            => hostBuilder.ConfigureServices((context, services) => services.AddSerialization());

    }
}
