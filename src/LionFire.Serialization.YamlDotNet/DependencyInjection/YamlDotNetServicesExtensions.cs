using LionFire.Serialization;
using LionFire.Serialization.Yaml.YamlDotNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using LionFire.Dependencies;
using Microsoft.Extensions.Hosting;
using LionFire.Applications.Hosting;
using YamlDotNet;


namespace LionFire.Services
{
    public static class YamlDotNetAppHostExtensions
    {
        public static IAppHost AddYamlDotNet(this IAppHost app) => app.TryAddEnumerableSingleton<ISerializationStrategy, YamlDotNetSerializer>();

        public static IHostBuilder AddYamlDotNet(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) => services.AddYamlDotNet());
            return hostBuilder;
        }

        public static IServiceCollection AddYamlDotNet(this IServiceCollection services)
        {
            //YamlDotNet.Core.EmitterSettings
            services
                .TryAddEnumerableSingleton<ISerializationStrategy, YamlDotNetSerializer>()
                .AddSingleton<YamlDotNetSerializer>()
                //.Configure<YamlSerializerSettings>(LionSerializeContext.Persistence.ToString(), c => c.SetDefaults(LionSerializeContext.Persistence))
                //.Configure<YamlSerializerSettings>(LionSerializeContext.Network.ToString(), c => c.SetDefaults(LionSerializeContext.Network))
                ;
            return services;
        }


        //public static YamlSerializerSettings SetDefaults(this YamlSerializerSettings settings, LionSerializeContext context)
        //{
        //    //settings.TypeNameHandling = TypeNameHandling.Auto;
        //    //settings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
        //    //settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;

        //    return settings;
        //}
    }
}
