using LionFire.Applications.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Extensions.Options
{
    public static class OptionsAppHostExtensions
    {
        public static string DefaultAppSettingsFileName = "appsettings";
        public static string DefaultAppSettingsFileExtension = "json";

        public static IAppHost AddOptions(this IAppHost app, string basePath, Action<IConfigurationBuilder> configureBuilder = null)
        {
            var builder = new ConfigurationBuilder()
                                    .SetBasePath(basePath);

            if (configureBuilder != null) { configureBuilder(builder); }
            else
            {
                builder.AddDefault();
            }

            var configRoot = builder.Build();
            Structures.ManualSingleton<IConfigurationRoot>.Instance = configRoot; // TEMP REVIEW
            app.Add(configRoot);
            return app;
        }

        public static IAppHost AddOptions(this IAppHost app, Action<IConfigurationBuilder> configureBuilder = null)
        {
            var builder = new ConfigurationBuilder();

            if (configureBuilder != null) { configureBuilder(builder); }
            else
            {
                builder.AddDefault();
            }

            var configRoot = builder.Build();
            app.Add(configRoot);
            return app;
        }

        public static IConfigurationBuilder AddDefault(this IConfigurationBuilder builder)
        {
            builder
                    .AddJsonFile($"{DefaultAppSettingsFileName}.{DefaultAppSettingsFileExtension}", optional: true, reloadOnChange: true)
            //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            ;
            return builder;
        }
    }
}
