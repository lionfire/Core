#nullable enable
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;


public class SubBuilders
{
    public Dictionary<Type, object> subBuilders=new();
    public T GetOrCreateSubBuilder<T>(Func<T> factory)
    {
        if (subBuilders.ContainsKey(typeof(T))) return (T)subBuilders[typeof(T)];
        var result = factory();
        if (result is { } notNullResult) subBuilders.Add(typeof(T), notNullResult);
        return result;
    }
}

public static class HostApplicationBuilderX
{
    public static HostApplicationBuilder CreateNonDefault(string[]? args = null, HostApplicationBuilderSettings? settings = null)
    {
        settings ??= new();
        settings.Args = args;
        settings.DisableDefaults = true;
        return new HostApplicationBuilder(settings);
    }

    /// <summary>
    /// Fluent builder for LionFire'Services initialization of HostApplicationBuilder
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="action"></param>
    /// <param name="useDefaults"></param>
    /// <returns></returns>
    public static HostApplicationBuilder LionFire(this HostApplicationBuilder hostBuilder, Action<ILionFireHostBuilder>? action = null, bool useDefaults = true, Action<ILionFireHostBuilder>? actionBeforeDefaults = null)
    {
        IHostApplicationBuilder hab = hostBuilder;

        #region SubBuilders: init

        SubBuilders SubBuilders;
        if (hab.Properties.ContainsKey("SubBuilders"))
        {
            SubBuilders = (SubBuilders)hab.Properties["SubBuilders"];
        }
        else
        {
            SubBuilders = new();
            hab.Properties.Add("SubBuilders", SubBuilders);
        }
        
        #endregion

        var lf = SubBuilders.GetOrCreateSubBuilder<LionFireHostBuilder>(() =>
        {
            var lf = new LionFireHostBuilder(hostBuilder); // TODO - reuse existing from Properties if it exists

            actionBeforeDefaults?.Invoke(lf);

            if (useDefaults) { lf.Defaults(); }

            return lf;
        });

        action?.Invoke(lf);

        return hostBuilder;
    }
    public static HostApplicationBuilder LionFire(this HostApplicationBuilder hostApplicationBuilder, int basePort, Action<ILionFireHostBuilder>? action = null, bool useDefaults = true, Action<ILionFireHostBuilder>? actionBeforeDefaults = null, bool useFallbackPorts = true)
        => hostApplicationBuilder
                .BasePort(basePort, useFallbackPorts)
                .LionFire(action, useDefaults, actionBeforeDefaults);
    

    // FUTURE - sort out IHostApplicationBuilder vs implementors such as HostApplicationBuilder (and nothing else so far?)
    //public static IHostApplicationBuilder LionFire(this IHostApplicationBuilder hostBuilder, int basePort, Action<ILionFireHostBuilder>? action = null, bool useDefaults = true)
    //{
    //    hostBuilder.LionFire(basePort, action, useDefaults);
    //    return hostBuilder;
    //}

    #region UseConsoleLifetime

    public static HostApplicationBuilder UseConsoleLifetime(this HostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services
            .AddSingleton<IHostLifetime, Microsoft.Extensions.Hosting.Internal.ConsoleLifetime>()
             ;
        return hostApplicationBuilder;
    }
    public static HostApplicationBuilder UseConsoleLifetime(this HostApplicationBuilder hostApplicationBuilder, Action<ConsoleLifetimeOptions> configureOptions)
    {
        hostApplicationBuilder.Services
            .AddSingleton<IHostLifetime, Microsoft.Extensions.Hosting.Internal.ConsoleLifetime>()
             .Configure(configureOptions)
             ;
        return hostApplicationBuilder;
    }

    #endregion





}
