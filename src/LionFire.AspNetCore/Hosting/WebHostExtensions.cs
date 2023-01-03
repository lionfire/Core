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

namespace LionFire.Hosting
{

    public static class WebHostExtensions
    {

        public static ILionFireHostBuilder WebHost<TStartup>(this ILionFireHostBuilder builder)
            where TStartup : class
            => builder.WebHost<TStartup>(null, false);

        public static ILionFireHostBuilder WebHost<TStartup>(this ILionFireHostBuilder builder, int? defaultPort, bool useReleaseChannelPortOffsets = true)
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

                        if (defaultPort.HasValue)
                        {
                            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#kestrel-endpoint-configuration
                            // https://andrewlock.net/5-ways-to-set-the-urls-for-an-aspnetcore-app/
                            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints?view=aspnetcore-6.0
                            webBuilder
                                .UseUrls($"http://localhost:{defaultPort.Value}")
                            ;
                        }
                    });

            return builder;
        }
    }
}
