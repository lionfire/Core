using LionFire.Deployment;
using LionFire.Hosting;
using LionFire.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace LionFire.Hosting;

/// <summary>
/// Allows for more dynamic configuration of Kestrel ports, based on ReleaseChannel
/// </summary>
public class WebHostOptions
{
    // REVIEW: consider IConfiguration approach instead:
    // .ConfigureHostConfiguration(b =>
    //    {
    //    b.AddInMemoryCollection(new Dictionary<string, string?>
    //    {
    //        ["Kestrel:Endpoints:Http:Url"] = $"http://{config.HttpInterface}:{config.HttpPort}",
    //    });

    public bool Enabled { get; set; } = true;

    /// <summary>
    /// This is in addition to the Urls provided when UseHttp or UseHttps are true.
    /// Default: none.
    /// e.g. new[] { "http://localhost:5000", "https://localhost:5001" }
    /// </summary>
    public List<string>? Urls { get; set; }

    #region Http

    public bool UseHttp { get; set; } = true;
    public List<string> HttpInterfaces { get; set; } = new() { "localhost" };
    public int? DefaultHttpPort { get; set; }

    #endregion

    #region Https
#if TODO
    public bool UseHttps { get; set; } = false;
    public List<string> HttpsInterfaces { get; set; } = new() { "localhost" };

    public int? DefaultHttpsPort { get; set; }
#endif
    #endregion

    #region Port from Release Channel

    public bool UseReleaseChannelPortOffsets { get; set; } = true;
    public string? ReleaseChannelOverride { get; set; }

    #endregion
}

public static class WebHostExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TStartup"></typeparam>
    /// <param name="builder"></param>
    /// <param name="defaultOptions">Overridden by Configuration</param>
    /// <returns></returns>
    public static ILionFireHostBuilder WebHost<TStartup>(this ILionFireHostBuilder builder, WebHostOptions? defaultOptions = null)
        where TStartup : class
        => builder.WebHost<TStartup>(null, defaultOptions);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TStartup"></typeparam>
    /// <param name="builder"></param>
    /// <param name="defaultHttpPort"></param>
    /// <param name="defaultOptions">Overridden by Configuration</param>
    /// <returns></returns>
    public static ILionFireHostBuilder WebHost<TStartup>(this ILionFireHostBuilder builder, int? defaultHttpPort, WebHostOptions? defaultOptions = null)
        where TStartup : class
    {
        var options = defaultOptions ?? new WebHostOptions();
        builder.HostBuilder.Configuration.GetSection("WebHost").Bind(options);

        if (!options.Enabled) return builder;

        defaultHttpPort ??= options.DefaultHttpPort;

        // TODO: Finish implementing HTTPS for WebHostOptions and eliminate this defaultHttpPort.HasValue logic
        if (defaultHttpPort.HasValue)
        {
            if (options.UseReleaseChannelPortOffsets == true)
            {
                var releaseChannel = options.ReleaseChannelOverride ?? builder.HostBuilder.Properties.TryGetValue("releaseChannel") as string;
                if (releaseChannel != null)
                {
                    var portOffset = DefaultReleaseChannels.TryGetReleaseChannelPortOffset(releaseChannel);
                    if (portOffset.HasValue)
                    {
                        defaultHttpPort += portOffset.Value;
                    }
                }
            }

            builder.HostBuilder
                .ConfigureServices((context, services) =>
                {
                    services.Configure<ServiceInfos>(services =>
                    {
                        services.Add(new PortServiceInfo(context.HostingEnvironment, context.Configuration) { Port = defaultHttpPort.Value });
                    });
                });

            // FUTURE: If defaultPort isn't used, what is the fallback?  Register that instead?
            //var server = services.GetService<IServer>();
            //var addressFeature = server.Features.Get<IServerAddressesFeature>();
            //foreach (var address in addressFeature.Addresses)
            //{
            //    _log.LogInformation("Listing on address: " + address);
            //}
        }

        builder.HostBuilder
            //.ConfigureServices((context, services) =>
            //    {
            //    // Reference: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/options?view=aspnetcore-6.0
            //        services.Configure<KestrelServerOptions>(
            //            context.Configuration.GetSection("Kestrel"));
            //    })
            .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        //.UseStartup(c => new TStartup())
                        .UseStartup<TStartup>()
                        .UseContentRoot(AppContext.BaseDirectory)
                    ;

                    // TODO: Finish implementing HTTPS for WebHostOptions and eliminate this defaultHttpPort.HasValue logic
                    if (defaultHttpPort.HasValue)
                    {
                        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#kestrel-endpoint-configuration
                        // https://andrewlock.net/5-ways-to-set-the-urls-for-an-aspnetcore-app/
                        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints?view=aspnetcore-6.0

                        List<string> urls = new();
                        if (options.UseHttp)
                        {
                            foreach (var iface in options.HttpInterfaces)
                            {
                                urls.Add($"http://{iface}:{defaultHttpPort.Value}");
                            }
                        }
                        //if (options.UseHttps) // TODO
                        //{
                        //    foreach (var iface in options.HttpsInterfaces)
                        //    {
                        //        urls.Add($"https://{iface}:{defaultHttpPort.Value}");
                        //    }
                        //}
                        if (options.Urls != null) { urls.AddRange(options.Urls); }

                        webBuilder
                            .UseUrls(urls.ToArray())
                        ;
                    }
                });

        return builder;
    }
}
