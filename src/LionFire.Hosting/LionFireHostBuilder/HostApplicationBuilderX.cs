#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace LionFire.Hosting;

public static class HostApplicationBuilderX
{
    /// <summary>
    /// Fluent builder for LionFire's initialization of HostApplicationBuilder
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="action"></param>
    /// <param name="useDefaults"></param>
    /// <returns></returns>
    public static HostApplicationBuilder LionFire(this HostApplicationBuilder hostBuilder, Action<ILionFireHostBuilder>? action = null, bool useDefaults = true)
    {
        var lf = new LionFireHostBuilder(hostBuilder);

        if (useDefaults) { lf.Defaults(); }

        action?.Invoke(lf);

        return hostBuilder;
    }

    public static HostApplicationBuilder ConfigureServices(this HostApplicationBuilder hostApplicationBuilder, Action<IServiceCollection> configure)
    {
        configure(hostApplicationBuilder.Services);
        return hostApplicationBuilder;
    }
}
