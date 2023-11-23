using LionFire.Serialization;
using LionFire.Serialization.Hjson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using LionFire.Dependencies;
using Microsoft.Extensions.Hosting;
using LionFire.Applications.Hosting;
using Hjson;

namespace LionFire.Hosting;


public static class HjsonAppHostExtensions
{
    public static IAppHost AddHjson(this IAppHost app) => app.TryAddEnumerableSingleton<ISerializationStrategy, HjsonSerializer>();

    public static IHostBuilder AddHjson(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices((context, services) => services.AddHjson());
        return hostBuilder;
    }

    public static IServiceCollection AddHjson(this IServiceCollection services)
    {
        services
            .TryAddEnumerableSingleton<ISerializationStrategy, HjsonSerializer>()
            .AddSingleton<HjsonSerializer>()
            .Configure<HjsonSerializerSettings>(LionSerializeContext.Persistence.ToString(), c => c.SetDefaults(LionSerializeContext.Persistence))
            .Configure<HjsonSerializerSettings>(LionSerializeContext.Network.ToString(), c => c.SetDefaults(LionSerializeContext.Network))
            ;
        return services;
    }


    public static HjsonSerializerSettings SetDefaults(this HjsonSerializerSettings settings, LionSerializeContext context)
    {
        //settings.TypeNameHandling = TypeNameHandling.Auto;
        ////Converters = ;
        //settings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
        ////NullValueHandling = 
        //settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;

        return settings;
    }
}
