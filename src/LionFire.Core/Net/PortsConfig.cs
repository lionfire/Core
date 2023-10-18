using LionFire.Configuration;
using Microsoft.Extensions.Configuration;

namespace LionFire.Net;

public class PortsConfig : IHasConfigLocation
{
    public static string DefaultConfigLocation => "Ports";
    public string ConfigLocation => DefaultConfigLocation;


    public PortsConfig(IConfiguration configuration)
    {
        this.Bind(configuration);
    }

    /// <summary>
    /// It's not recommended to use this port, outside of trying samples.  The idea is that if you see this port number, it means you forgot to configure the port.
    /// </summary>
    public const int DefaultPortBase = 5000;


    public int? BasePort { get; set; } = DefaultPortBase;

    #region ENH: Release channel offsets from BasePort

    //public bool UseReleaseChannelPortOffsets { get; set; } = true;
    //public string? ReleaseChannelOverride { get; set; }

    #endregion
}

#if false
void OldReleaseChannelPortCode()
        {
            if (OLD_OPTIONS.UseReleaseChannelPortOffsets == true)
            {
                var releaseChannel = OLD_OPTIONS.ReleaseChannelOverride ?? builder.HostBuilder.Properties.TryGetValue("releaseChannel") as string;
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

#endif