using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace LionFire.Hosting;

// TODO: Refactor this with LionFireHostBuilder, support other builder types: ASP.NET Core, MAUI

public class HostBuilderWrapper : IHostBuilder
{

    public HostBuilderWrapper(IHostBuilder hostBuilder, ILionFireHostBuilder parent)
    {
        WrappedHostBuilder = hostBuilder;
        Parent = parent;
    }
    public HostBuilderWrapper(HostApplicationBuilder hostApplicationBuilder, ILionFireHostBuilder parent)
    {
        WrappedHostApplicationBuilder = hostApplicationBuilder;
        //WrappedHostBuilder = new HostBuilderAdapter(WrappedHostApplicationBuilder); // Exists internally to Microsoft code
        Parent = parent;
    }
    public IHostBuilder WrappedHostBuilder { get; set; }
    public HostApplicationBuilder WrappedHostApplicationBuilder { get; set; }

    public ILionFireHostBuilder Parent { get; }

    public ILionFireHostBuilder Done() => Parent;

    Exception NotSupportedForHostBuilderException => new NotSupportedException($"Not supported for {nameof(IHostBuilder)}");
    public IConfiguration Configuration => WrappedHostApplicationBuilder?.Configuration ?? throw NotSupportedForHostBuilderException; // TODO: How to get Configuration? Build first?
    //(WrappedHostBuilder.Build().Services.GetService<IConfiguration>());

    public IConfigurationBuilder ConfigurationBuilder => WrappedHostApplicationBuilder?.Configuration ?? throw NotSupportedForHostBuilderException; // TODO: How to get IConfigurationBuilder?

    public IDictionary<object, object> Properties => WrappedHostBuilder != null ? WrappedHostBuilder.Properties : (properties ??= new());
    public Dictionary<object, object> properties;

    public IHost Build() => WrappedHostBuilder?.Build() ?? WrappedHostApplicationBuilder.Build();

    public HostBuilderContext WrappedHostApplicationBuilderContext 
        => new(Properties)
                {
                    Configuration = WrappedHostApplicationBuilder.Configuration,
                    HostingEnvironment = WrappedHostApplicationBuilder.Environment,
                };

    public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        if (WrappedHostBuilder != null)
        {
            WrappedHostBuilder.ConfigureAppConfiguration(configureDelegate);
        }
        else if (WrappedHostApplicationBuilder != null)
        {
            configureDelegate(WrappedHostApplicationBuilderContext, WrappedHostApplicationBuilder.Configuration);
        }
        else throw NoHostBuilderException;

        return this;
    }

    public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate) => WrappedHostBuilder.ConfigureContainer(configureDelegate);

    public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
    {
        if (WrappedHostBuilder != null)
        {
            WrappedHostBuilder.ConfigureHostConfiguration(configureDelegate);
        }
        else if (WrappedHostApplicationBuilder != null)
        {
            configureDelegate(WrappedHostApplicationBuilder.Configuration);
        }
        else throw NoHostBuilderException;

        return this;
    }

    public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        if (WrappedHostBuilder != null)
        {
            WrappedHostBuilder.ConfigureServices(configureDelegate);
        }
        else if (WrappedHostApplicationBuilder != null)
        {
            configureDelegate(WrappedHostApplicationBuilderContext, WrappedHostApplicationBuilder.Services); // DANGER: will throw at runtime if caller needs HostBuilderContext.
        }
        else throw NoHostBuilderException;

        return this;
    }

    public IHostBuilder ConfigureServices(Action<IServiceCollection> configureDelegate)
    {
        if (WrappedHostBuilder != null)
        {
            WrappedHostBuilder.ConfigureServices(configureDelegate);
        }
        else if (WrappedHostApplicationBuilder != null)
        {
            configureDelegate(WrappedHostApplicationBuilder.Services);
        }
        else throw NoHostBuilderException;

        return this;
    }

    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
    {
        if (WrappedHostBuilder != null) WrappedHostBuilder.UseServiceProviderFactory(factory);
        else WrappedHostApplicationBuilder.ConfigureContainer(factory);
        return this;
    }

    /// <summary>
    /// Only supported by HostApplicationBuilder
    /// </summary>
    /// <typeparam name="TContainerBuilder"></typeparam>
    /// <param name="factory"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder> configure)
    {
        WrappedHostApplicationBuilder.ConfigureContainer(factory, configure);
        return this;
    }

    /// <summary>
    /// Only supported by IHostBuilder
    /// </summary>
    /// <typeparam name="TContainerBuilder"></typeparam>
    /// <param name="factory"></param>
    /// <returns></returns>
    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) => WrappedHostBuilder.UseServiceProviderFactory(factory);

    #region Exceptions

    Exception NoHostBuilderException => new ArgumentNullException($"One of these must be set: {nameof(WrappedHostBuilder)}, {nameof(WrappedHostApplicationBuilder)}");


    #endregion
}
