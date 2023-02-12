#nullable enable
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace LionFire.Hosting;

public interface ILionFireHostBuilder
{
    HostBuilderWrapper HostBuilder { get; }

    ILionFireHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configure);
    ILionFireHostBuilder ConfigureServices(Action<IServiceCollection> configure);
    ILionFireHostBuilder ForHostBuilder(Action<IHostBuilder> action);
    IConfiguration GetBootstrapConfiguration(LionFireHostBuilderBootstrapOptions? options = null);
}