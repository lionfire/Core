#nullable enable
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

    IConfiguration Configuration => HostBuilder.Configuration;

    IConfigurationManager ConfigurationManager => IHostApplicationBuilder.Configuration;

    ILionFireHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configure);
    ILionFireHostBuilder ConfigureServices(Action<IServiceCollection> configure);
    ILionFireHostBuilder ForHostBuilder(Action<IHostBuilder> action);
    //ILionFireHostBuilder ForHostApplicationBuilder(Action<HostApplicationBuilder> action);
    ILionFireHostBuilder ForIHostApplicationBuilder(Action<IHostApplicationBuilder> action);
    IConfiguration GetBootstrapConfiguration(LionFireHostBuilderBootstrapOptions? options = null);

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

        hab.Configuration.AddInMemoryCollection([new($"{PortsConfig.DefaultConfigLocation}:{nameof(PortsConfig.BasePort)}", port.ToString())]);
        hab.Configuration.AddInMemoryCollection([new($"{PortsConfig.DefaultConfigLocation}:{nameof(PortsConfig.UseFallbackPorts)}", useFallbackPorts.ToString())]);

        hab.Configuration.AddInMemoryCollection([new($"{PortsConfig.DefaultConfigLocation}:{nameof(PortsConfig.EffectiveBasePort)}",
                (useFallbackPorts
                    ? GetEffectiveBasePort(port, portCount)
                    : port
                    ).ToString())]
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
        var usedPorts = new List<int>();

        for (int port = startPort; port < endPortExclusive; port++)
        {
            if (tcpConnections.Any(c => c.LocalEndPoint.Port == port) ||
                tcpListeners.Any(l => l.Port == port) ||
                udpListeners.Any(l => l.Port == port))
            {
                return false;
                //usedPorts.Add(port);
            }
        }
        return true;
        //return usedPorts;
    }

}