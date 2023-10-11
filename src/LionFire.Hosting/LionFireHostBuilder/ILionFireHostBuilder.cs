#nullable enable
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace LionFire.Hosting;

public interface ILionFireHostBuilder
{
    LionFireHostBuilderWrapper HostBuilder { get; }

    ILionFireHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configure);
    ILionFireHostBuilder ConfigureServices(Action<IServiceCollection> configure);
    ILionFireHostBuilder ForHostBuilder(Action<IHostBuilder> action);
    IConfiguration GetBootstrapConfiguration(LionFireHostBuilderBootstrapOptions? options = null);
}

public static class ILionFireHostBuilderX
{
    public static ILionFireHostBuilder ConfigureDefaults(this ILionFireHostBuilder lf, params KeyValuePair<string, string?>[] kvps)
    => lf.ForHostBuilder(b => b.ConfigureHostConfiguration(c => c.AddInMemoryCollection(kvps)));
}