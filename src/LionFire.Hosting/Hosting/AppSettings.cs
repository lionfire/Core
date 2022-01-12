using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Hosting
{


    public static class AppSettings
    {
        public static LionFireHostBuilder CopyExampleAppSettings(this LionFireHostBuilder builder)
        {
            builder.HostBuilder.ConfigureAppConfiguration((hostContext, config) =>
            {
                var releaseChannel = hostContext.Configuration["releaseChannel"];
                LionFireHostingConfigurationUtils.OverwriteFromBaseDirectory("example.appsettings.json");

                if (releaseChannel != null)
                {
                    LionFireHostingConfigurationUtils.OverwriteFromBaseDirectory($"example.appsettings.{releaseChannel}.json");
                }
                //foreach (var channelId in ReleaseChannelIds)
                //{
                //    LionFireHostingConfigurationUtils.TryCopyFromBaseDirectoryIfMissing($"appsettings.{channelId}.json");
                //}
            });
            return builder;
        }

        //public static LionFireHostBuilder CopyMissingAppSettings(this LionFireHostBuilder builder)
        //{
        //    builder.HostBuilder.ConfigureAppConfiguration((hostContext, config) =>
        //    {
        //        var releaseChannel = hostContext.Configuration["releaseChannel"];
        //        LionFireHostingConfigurationUtils.TryCopyFromBaseDirectoryIfMissing("appsettings.json", "appsettings.example.json");

        //        if (releaseChannel != null)
        //        {
        //            LionFireHostingConfigurationUtils.TryCopyFromBaseDirectoryIfMissing($"appsettings.{releaseChannel}.json");
        //        }
        //        //foreach (var channelId in ReleaseChannelIds)
        //        //{
        //        //    LionFireHostingConfigurationUtils.TryCopyFromBaseDirectoryIfMissing($"appsettings.{channelId}.json");
        //        //}
        //    });
        //    return builder;
        //}

        public static LionFireHostBuilder ReleaseChannel(this LionFireHostBuilder builder, bool reloadOnChange = true)
        {
            builder.HostBuilder.AddHostedService<ReleaseChannelLogger>();

            var releaseChannel = Environment.GetEnvironmentVariable("RELEASE_CHANNEL");
            if (releaseChannel != null)
            {
                builder.HostBuilder.Properties.Add("releaseChannel", releaseChannel);
            }

            builder.HostBuilder.ConfigureHostConfiguration(config =>
            {
                if (releaseChannel != null)
                {
                    config.AddInMemoryCollection(new KeyValuePair<string, string>[] { new("releaseChannel", releaseChannel) });
                }
            });

            builder.HostBuilder.ConfigureAppConfiguration((hostContext, config) =>
            {
                var releaseChannel = hostContext.Configuration["releaseChannel"];
                if (releaseChannel != null)
                {
                    config
                        .AddJsonFile($"appsettings.{releaseChannel}.json", optional: true, reloadOnChange)
                        ;
                }
            });

            return builder;
        }


    }
}
