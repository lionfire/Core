﻿using LionFire.Configuration.ReleaseChannels;
using LionFire.Deployment;
using LionFire.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace LionFire.Hosting;

public static class ReleaseChannelsHostingX
{
    public static HostApplicationBuilder ReleaseChannel(this HostApplicationBuilder hostBuilder, bool reloadOnChange = true)
    {
        hostBuilder.Services.AddHostedService<ReleaseChannelLogger>();

        // Old, use DOTNET_releaseChannel
        //var releaseChannel = Environment.GetEnvironmentVariable("RELEASE_CHANNEL");

        //if (releaseChannel != null)
        //{
        //    hostBuilder.Configuration
        //            .AddInMemoryCollection(new KeyValuePair<string, string>[] { new(ReleaseChannelKeys.ReleaseChannel, releaseChannel) });
        //}

        var configuredReleaseChannel = hostBuilder.Configuration[ReleaseChannelKeys.ReleaseChannel];
        if (configuredReleaseChannel != null)
        {
            var releaseChannelConfigFile = $"appsettings.{configuredReleaseChannel}.json";

            Log.Get(typeof(ReleaseChannelsHostingX).FullName).LogInformation($"Adding configuration source for release channel: {releaseChannelConfigFile}");

            hostBuilder.Configuration.AddJsonFile(releaseChannelConfigFile, optional: true, reloadOnChange);
        }

        return hostBuilder;
    }

    public static IHostBuilder ReleaseChannel(this IHostBuilder hostBuilder, bool reloadOnChange = true)
    {
        hostBuilder.ConfigureServices(s => s.AddHostedService<ReleaseChannelLogger>());

        var releaseChannel = Environment.GetEnvironmentVariable("RELEASE_CHANNEL");
        if (releaseChannel != null)
        {
            hostBuilder.Properties.Add("releaseChannel", releaseChannel);
        }

        hostBuilder.ConfigureHostConfiguration(config =>
        {
            if (releaseChannel != null)
            {
                config.AddInMemoryCollection(new KeyValuePair<string, string>[] { new("releaseChannel", releaseChannel) });
            }
        });

        hostBuilder.ConfigureAppConfiguration((hostContext, config) =>
        {
            var releaseChannel = hostContext.Configuration["releaseChannel"];
            if (releaseChannel != null)
            {
                config
                    .AddJsonFile($"appsettings.{releaseChannel}.json", optional: true, reloadOnChange)
                    ;
            }
        });

        return hostBuilder;
    }
}