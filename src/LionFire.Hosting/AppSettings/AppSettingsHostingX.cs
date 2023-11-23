using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class AppSettingsHostingX
{
    //public static ILionFireHostBuilder CopyMissingAppSettings(this ILionFireHostBuilder builder)
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

    public static IHostBuilder CopyExampleAppSettings(this IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((hostContext, config) =>
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

    public static IHostApplicationBuilder CopyExampleAppSettings(this IHostApplicationBuilder builder)
    {
        var releaseChannel = builder.Configuration["releaseChannel"];
        LionFireHostingConfigurationUtils.OverwriteFromBaseDirectory("example.appsettings.json");

        if (releaseChannel != null)
        {
            LionFireHostingConfigurationUtils.OverwriteFromBaseDirectory($"example.appsettings.{releaseChannel}.json");
        }
        //foreach (var channelId in ReleaseChannelIds)
        //{
        //    LionFireHostingConfigurationUtils.TryCopyFromBaseDirectoryIfMissing($"appsettings.{channelId}.json");
        //}
        return builder;
    }

}
