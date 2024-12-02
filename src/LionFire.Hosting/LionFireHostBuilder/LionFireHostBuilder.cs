#nullable enable
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
/// Marker type for fluent building of programs that want to opt in to LionFire'Services common program features and opinionated conventions
/// </summary>
public class LionFireHostBuilder : HostApplicationSubBuilder, ILionFireHostBuilder
{
    public LionFireHostBuilder(IHostBuilder hostBuilder) { HostBuilder = new LionFireHostBuilderWrapper(hostBuilder, this); }
    public LionFireHostBuilder(HostApplicationBuilder hostApplicationBuilder) { HostBuilder = new LionFireHostBuilderWrapper(hostApplicationBuilder, this); }
    public LionFireHostBuilder(IHostApplicationBuilder hostApplicationBuilder) { HostBuilder = new LionFireHostBuilderWrapper(hostApplicationBuilder, this); }


    public LionFireHostBuilderWrapper HostBuilder { get; }
    public override IHostApplicationBuilder IHostApplicationBuilder => HostBuilder.WrappedIHostApplicationBuilder;

    public ILionFireHostBuilder ForHostBuilder(Action<IHostBuilder> action)
    {
        action(HostBuilder);
        return this;
    }

    //[Obsolete("Use ForIHostApplicationBuilder instead")]
    //public ILionFireHostBuilder ForHostApplicationBuilder(Action<HostApplicationBuilder> action)
    //{
    //    if (HostBuilder.WrappedHostApplicationBuilder == null) throw new NotSupportedException($"{typeof(HostApplicationBuilder).Name} not available");
    //    action(HostBuilder.WrappedHostApplicationBuilder);
    //    return this;
    //}

    public ILionFireHostBuilder ForIHostApplicationBuilder(Action<IHostApplicationBuilder> action)
    {
        if (HostBuilder.WrappedIHostApplicationBuilder == null) throw new NotSupportedException($"{typeof(IHostApplicationBuilder).Name} not available");
        action(HostBuilder.WrappedIHostApplicationBuilder);
        return this;
    }

    public ILionFireHostBuilder ConfigureServices(Action<IServiceCollection> configure) { HostBuilder.ConfigureServices(configure); return this; }
    public ILionFireHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configure) { HostBuilder.ConfigureServices(configure); return this; }

    public IConfiguration GetConfigurationForLogBootstrap(LionFireHostBuilderBootstrapOptions? options = null)
    {
        if (HostBuilder.WrappedIHostApplicationBuilder != null)
        {
            return HostBuilder.WrappedIHostApplicationBuilder.Configuration;
        }
        else
        {
            var hab = new HostApplicationBuilder();
            options ??= new();
            hab.LionFire(options.BootstrapLionFireHostBuilder, options.UseDefaults);

            return hab.Configuration;
        }
    }

}

#if UNUSED // Obsolete - use ForHostBuilder instead
public static class DoneExtensions
{
    public static ILionFireHostBuilder Done(this IHostBuilder hostBuilder) => (hostBuilder as LionFireHostBuilderWrapper)?.Done()
        ?? throw new ArgumentException("hostBuilder is not LionFireHostBuilderWrapper and does not need Done")
        ;
}
#endif