using LionFire.Deployment;
using LionFire.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace LionFire.Hosting;

// TODO: Refactor common code between IHostBuilder and IHostApplicationBuilder
public static class ReleaseChannelsHostingX
{
    public static string TestReleaseChannel = "test";

    public static IHostApplicationBuilder DeploymentSlot(this IHostApplicationBuilder hostBuilder, bool reloadOnChange = true)
    {
        hostBuilder.Services.AddHostedService<DeploymentSlotLogger>();

        var value = _EnvVar(hostBuilder, "slot");

        if (value != null)
        {
            var configFile = $"appsettings.slot.{value}.json";
            Log.Get(typeof(ReleaseChannelsHostingX).FullName).LogInformation($"Adding configuration source for deployment slot: {configFile}");
            hostBuilder.Configuration.AddJsonFile(configFile, optional: true, reloadOnChange);
        }

        return hostBuilder;
    }

    private static string _EnvVar(IHostApplicationBuilder hostBuilder, string name, bool reloadOnChange = true)
    {
        var value = hostBuilder.Configuration[name];
        return value;
    }
        
    public static IHostApplicationBuilder ReleaseChannel(this IHostApplicationBuilder hostBuilder, bool reloadOnChange = true)
    {
        hostBuilder.Services.AddHostedService<ReleaseChannelLogger>();

        // REVIEW - not needed?
        //if (value != null)
        //{
        //    hostBuilder.Configuration
        //            .AddInMemoryCollection(new KeyValuePair<string, string>[] { new(ReleaseChannelKeys.ReleaseChannel, value) });
        //}

        var value = _EnvVar(hostBuilder, ReleaseChannelKeys.ReleaseChannel);

        if (value == null && LionFireEnvironment.IsUnitTest == true)
        {
            hostBuilder.Configuration
                    .AddInMemoryCollection(new KeyValuePair<string, string>[] { new(ReleaseChannelKeys.ReleaseChannel, TestReleaseChannel) });
        }

        if (value != null)
        {
            var configFile = $"appsettings.{value}.json";
            Log.Get(typeof(ReleaseChannelsHostingX).FullName).LogInformation($"Adding configuration source for release channel: {configFile}");
            hostBuilder.Configuration.AddJsonFile(configFile, optional: true, reloadOnChange);
        }

        return hostBuilder;
    }

    

    static string configDirFromEnv => Environment.GetEnvironmentVariable($"DOTNET_ConfigDir");
    static string configDir => configDirFromEnv ?? Environment.CurrentDirectory;

    public static IHostBuilder DeploymentSlot(this IHostBuilder hostBuilder, bool reloadOnChange = true)
    {
        hostBuilder.ConfigureServices(s => s.AddHostedService<DeploymentSlotLogger>());

        var value = _EnvVar(hostBuilder, "Slot");

        hostBuilder.ConfigureHostConfiguration(config =>
        {
            if (value != null)
            {
                config
                    .AddJsonFile(Path.Combine(configDir, $"appsettings.slot.{value}.json"), optional: true, reloadOnChange)
                ;
            }
        });

        return hostBuilder;
    }

    private static string _EnvVar(this IHostBuilder hostBuilder, string name, bool reloadOnChange = true)
    {
        var value = Environment.GetEnvironmentVariable($"DOTNET_{name}");

        if (value != null) { hostBuilder.Properties.Add(name, value); }

        hostBuilder.ConfigureHostConfiguration(config =>
        {
            if (value != null)
            {
                config
                    .AddInMemoryCollection(new KeyValuePair<string, string>[] { new(name, value) })
                ;
            }
        });
        return value;
    }
    public static IHostBuilder ReleaseChannel(this IHostBuilder hostBuilder, bool reloadOnChange = true)
    {
        hostBuilder.ConfigureServices(s => s.AddHostedService<ReleaseChannelLogger>());

        var value = _EnvVar(hostBuilder, "ReleaseChannel");
        if (value == null && LionFireEnvironment.IsUnitTest == true) value = TestReleaseChannel;

        hostBuilder.ConfigureHostConfiguration(config =>
        {
            if (value != null)
            {
                config
                    .AddJsonFile(Path.Combine(configDir, $"appsettings.{value}.json"), optional: true, reloadOnChange)
                ;
            }
        });

        return hostBuilder;
    }
}