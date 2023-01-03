using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace LionFire.Hosting;

public class LionFireHostBuilderBootstrapOptions
{
    public bool UseDefaults { get; set; } = true;
    public Action<ILionFireHostBuilder>? BootstrapLionFireHostBuilder { get; set; }
}

/// <summary>
/// Marker type for fluent building of programs that want to opt in to LionFire's common program features and opinionated conventions
/// </summary>
public class LionFireHostBuilder : ILionFireHostBuilder
{
    public LionFireHostBuilder(IHostBuilder hostBuilder) { HostBuilder = new HostBuilderWrapper(hostBuilder, this); }
    public LionFireHostBuilder(HostApplicationBuilder hostApplicationBuilder) { HostBuilder = new HostBuilderWrapper(hostApplicationBuilder, this); }

    public HostBuilderWrapper HostBuilder { get; }

    public ILionFireHostBuilder ForHostBuilder(Action<IHostBuilder> action)
    {
        action(HostBuilder);
        return this;
    }

    public ILionFireHostBuilder ConfigureServices(Action<IServiceCollection> configure) { HostBuilder.ConfigureServices(configure); return this; }
    public ILionFireHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configure) { HostBuilder.ConfigureServices(configure); return this; }

    public IConfiguration GetBootstrapConfiguration(LionFireHostBuilderBootstrapOptions options)
    {
        if (HostBuilder.WrappedHostApplicationBuilder != null)
        {
            return HostBuilder.WrappedHostApplicationBuilder.Configuration;
        }
        else
        {
            var hab = new HostApplicationBuilder();

            hab.LionFire(options.BootstrapLionFireHostBuilder, options.UseDefaults);

            return hab.Configuration;
        }
    }

}

public static class DoneExtensions
{
    public static ILionFireHostBuilder Done(this IHostBuilder hostBuilder) => (hostBuilder as HostBuilderWrapper)?.Done() 
        ?? throw new ArgumentException("hostBuilder is not HostBuilderWrapper and does not need Done")
        ;
}
