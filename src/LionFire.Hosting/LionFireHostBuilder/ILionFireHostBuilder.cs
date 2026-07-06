#nullable enable
using LionFire.Deployment;
using LionFire.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace LionFire.Hosting;

// FUTURE: Remove support for IHostBuilder, and rely solely on IHostApplicationBuilder?
public interface ILionFireHostBuilder : IHostApplicationSubBuilder
{
    LionFireHostBuilderWrapper HostBuilder { get; }

    Type? UnderlyingHostType
    {
        get
        {
            if (HostBuilder.WrappedIHostApplicationBuilder != null) { return HostBuilder.WrappedIHostApplicationBuilder.GetType(); }
            if (HostBuilder.WrappedHostBuilder != null) { return HostBuilder.WrappedHostBuilder.GetType(); }
            //if(HostBuilder.WrappedHostBuilder != null) { return typeof(IHostBuilder); }
            return null;
        }
    }

    IConfiguration Configuration => HostBuilder.Configuration;

    IConfigurationManager ConfigurationManager => IHostApplicationBuilder.Configuration;

    ILionFireHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configure);
    ILionFireHostBuilder ConfigureServices(Action<IServiceCollection> configure);
    ILionFireHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> action);
    ILionFireHostBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> action);

    ILionFireHostBuilder ForHostBuilder(Action<IHostBuilder> action);
    //ILionFireHostBuilder ForHostApplicationBuilder(Action<HostApplicationBuilder> action);
    ILionFireHostBuilder ForIHostApplicationBuilder(Action<IHostApplicationBuilder> action);
    IConfiguration GetConfigurationForLogBootstrap(LionFireHostBuilderBootstrapOptions? options = null);

}

public static class ILionFireHostBuilderX
{
    public static ILionFireHostBuilder ConfigureDefaults(this ILionFireHostBuilder lf, params KeyValuePair<string, string?>[] keyValuePairs)
    => lf.ForHostBuilder(b => b.ConfigureHostConfiguration(c => c.AddInMemoryCollection(keyValuePairs)));

    public static ILionFireHostBuilder Configure(this ILionFireHostBuilder lf, Action<IHostEnvironment, IConfigurationBuilder> configure)
      => lf.ForIHostApplicationBuilder(b => configure(b.Environment, b.Configuration));

    public static ILionFireHostBuilder BasePort(this ILionFireHostBuilder lf, int port)
    {
        lf.ConfigureDefaults([new($"{PortsConfig.DefaultConfigLocation}:BasePort", port.ToString())]);
        throw new NotImplementedException("TODO: refactor to use implementation for HostApplicationBuilder");
        //return lf;
    }

    public static HostApplicationBuilder BasePort(this HostApplicationBuilder hab, int port, bool useFallbackPorts = true, int portCount = 10)
    {
        hab.Services.ConfigurePorts(hab.Configuration);

        // Release-channel port offset: each deployment channel gets its own block of ports
        // (Precedence * PortBlockSize) from the nominal base, so multiple channels can run on one host.
        // The channel id is read from the env var directly so this works before .ReleaseChannel() runs.
        var channelId = hab.Configuration[ReleaseChannelKeys.ReleaseChannel]
            ?? Environment.GetEnvironmentVariable($"DOTNET_{ReleaseChannelKeys.ReleaseChannel}");
        var channel = string.IsNullOrEmpty(channelId) ? null : ReleaseChannels.TryParse(channelId);
        var channelOffset = channel is null ? 0 : channel.Precedence * ReleaseChannels.PortBlockSize;
        var channeledBasePort = port + channelOffset;

        hab.Configuration.AddInMemoryCollection([new($"{PortsConfig.DefaultConfigLocation}:{nameof(PortsConfig.BasePort)}", channeledBasePort.ToString())]);
        hab.Configuration.AddInMemoryCollection([new($"{PortsConfig.DefaultConfigLocation}:{nameof(PortsConfig.UseFallbackPorts)}", useFallbackPorts.ToString())]);

        // When a release channel is explicitly set, ports are deterministic (nominal base + channel block)
        // so dev/prod always land on known ports — even for offset-0 channels like Prod/Green (which must
        // NOT fall into port probing). Only probe for a free block when no channel is in play.
        var effectiveBasePort = (channel != null || !useFallbackPorts)
            ? channeledBasePort
            : GetEffectiveBasePort(channeledBasePort, portCount);

        hab.Configuration.AddInMemoryCollection([new($"{PortsConfig.DefaultConfigLocation}:{nameof(PortsConfig.EffectiveBasePort)}",
                effectiveBasePort.ToString())]
            );

        return hab;
    }

    private static int GetEffectiveBasePort(int port, int portCount = 10)
    {
        var ports = new LocalPortScanner();
        int probePort = 0;
        bool isFree() => probePort >= 1024 && ports.ArePortsFree(probePort, probePort + portCount);

        for (int modulus = 0; modulus < 1000; modulus += 100)
        {
            probePort = port + modulus;

            for (; probePort < (65535 - portCount); probePort += 10_000)
            {
                if (isFree()) return probePort;
            }
            for (probePort = (port + modulus) % 10_000; probePort < (65535 - portCount); probePort += 10_000)
            {
                if (isFree()) return probePort;
            }
        }

        int maxAttempts = 50000;

        while (maxAttempts-- > 0)
        {
            probePort = Random.Shared.Next(1000, 65535 - portCount);
            if (isFree()) return probePort;
        }
        throw new Exception($"Failed to find an unused port range for {portCount} ports");
    }
}

public class LocalPortScanner
{
    TcpConnectionInformation[] tcpConnections;
    System.Net.IPEndPoint[] tcpListeners;
    System.Net.IPEndPoint[] udpListeners;

    public LocalPortScanner()
    {
        var properties = IPGlobalProperties.GetIPGlobalProperties();
        tcpConnections = properties.GetActiveTcpConnections();
        tcpListeners = properties.GetActiveTcpListeners();
        udpListeners = properties.GetActiveUdpListeners();
    }

    public bool ArePortsFree(int startPort, int endPortExclusive)
    {
        for (int port = startPort; port < endPortExclusive; port++)
        {
            // Only check active listeners and established connections.
            // TIME_WAIT connections are safe to rebind with SO_REUSEADDR.
            if (tcpConnections.Any(c => c.LocalEndPoint.Port == port && c.State != TcpState.TimeWait) ||
                tcpListeners.Any(l => l.Port == port) ||
                udpListeners.Any(l => l.Port == port))
            {
                return false;
            }
        }
        return true;
    }

}