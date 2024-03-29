﻿#nullable enable
using LionFire.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

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
    public static ILionFireHostBuilder ConfigureDefaults(this ILionFireHostBuilder lf, params KeyValuePair<string, string?>[] kvps)
    => lf.ForHostBuilder(b => b.ConfigureHostConfiguration(c => c.AddInMemoryCollection(kvps)));

    public static ILionFireHostBuilder Configure(this ILionFireHostBuilder lf, Action<IHostEnvironment, IConfigurationBuilder> configure)
      => lf.ForIHostApplicationBuilder(b => configure(b.Environment, b.Configuration));

    public static ILionFireHostBuilder BasePort(this ILionFireHostBuilder lf, int port)
        => lf.ConfigureDefaults([new($"{PortsConfig.DefaultConfigLocation}:BasePort", port.ToString())]);

    public static HostApplicationBuilder BasePort(this HostApplicationBuilder hab, int port)
    { hab.Configuration.AddInMemoryCollection([new($"{PortsConfig.DefaultConfigLocation}:BasePort", port.ToString())]); return hab; }

}