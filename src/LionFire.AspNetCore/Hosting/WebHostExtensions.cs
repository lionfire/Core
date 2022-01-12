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
using NLog.Fluent;
using System;
using System.Collections.Generic;

namespace LionFire.Hosting
{

    public static class WebHostExtensions
    {
        public static LionFireHostBuilder WebHost<TStartup>(this LionFireHostBuilder builder, int? defaultPort = null, bool useReleaseChannelPortOffsets = true)
            where TStartup : class
        {
            if (defaultPort.HasValue)
            {
                if (useReleaseChannelPortOffsets)
                {
                    var releaseChannel = builder.HostBuilder.Properties.TryGetValue("releaseChannel") as string;
                    if (releaseChannel != null)
                    {
                        var portOffset = DefaultReleaseChannels.TryGetReleaseChannelPortOffset(releaseChannel);
                        if (portOffset.HasValue)
                        {
                            defaultPort += portOffset.Value;
                        }
                    }
                }

                builder.HostBuilder
                    .ConfigureServices((context, services) =>
                    {
                        services.Configure<ServiceInfos>(services =>
                        {
                            services.Add(new PortServiceInfo(context.HostingEnvironment, context.Configuration) { Port = defaultPort.Value });
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
        .ConfigureServices((context, services) =>
            {
                    // Reference: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/options?view=aspnetcore-6.0
                    services.Configure<KestrelServerOptions>(
                    context.Configuration.GetSection("Kestrel"));
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
        webBuilder.UseStartup<TStartup>();
                


        if (defaultPort.HasValue)
        {
            webBuilder
                .UseUrls($"http://localhost:{defaultPort.Value}")
                .UseContentRoot(AppContext.BaseDirectory)
                ;
        }
    });

            return builder;
        }
    }
}
