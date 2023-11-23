#nullable enable
using LionFire.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace LionFire.Hosting;

public interface ILionFireHostBuilder
{
    LionFireHostBuilderWrapper HostBuilder { get; }

    IConfiguration Configuration => HostBuilder.Configuration;

    ILionFireHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configure);
    ILionFireHostBuilder ConfigureServices(Action<IServiceCollection> configure);
    ILionFireHostBuilder ForHostBuilder(Action<IHostBuilder> action);
    //ILionFireHostBuilder ForHostApplicationBuilder(Action<HostApplicationBuilder> action);
    ILionFireHostBuilder ForIHostApplicationBuilder(Action<IHostApplicationBuilder> action);
    IConfiguration GetBootstrapConfiguration(LionFireHostBuilderBootstrapOptions? options = null);

}

public static class ILionFireHostBuilderX
{
    public static ILionFireHostBuilder ConfigureDefaults(this ILionFireHostBuilder lf, params KeyValuePair<string, string?>[] kvps)
    => lf.ForHostBuilder(b => b.ConfigureHostConfiguration(c => c.AddInMemoryCollection(kvps)));

    public static ILionFireHostBuilder BasePort(this ILionFireHostBuilder lf, int port)
        => lf.ConfigureDefaults([ new($"{PortsConfig.DefaultConfigLocation }:BasePort", port.ToString()) ]);

    public static HostApplicationBuilder BasePort(this HostApplicationBuilder hab, int port)
    { hab.Configuration.AddInMemoryCollection([new($"{PortsConfig.DefaultConfigLocation}:BasePort", port.ToString())]); return hab; }
}