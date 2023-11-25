using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using static LionFire.Hosting.HostApplicationBuilderToIHostBuilderAdapter;

namespace LionFire.Hosting;

// TODO: Switch to Microsoft'Services internal HostBuilderAdapter?
public class HostApplicationBuilderToIHostBuilderAdapter : IHostBuilder
{
    #region Relationships

    public HostApplicationBuilder WrappedHostApplicationBuilder { get; }

    #endregion

    #region Lifecycle

    public HostApplicationBuilderToIHostBuilderAdapter(HostApplicationBuilder hostApplicationBuilder)
    {
        WrappedHostApplicationBuilder = hostApplicationBuilder ?? throw new ArgumentNullException();
    }

    #endregion

    Exception NotSupportedForHostBuilderException => new NotSupportedException($"Not supported for {nameof(IHostBuilder)}");

    public HostBuilderContext WrappedHostApplicationBuilderContext
        => new(Properties)
        {
            Configuration = WrappedHostApplicationBuilder.Configuration,
            HostingEnvironment = WrappedHostApplicationBuilder.Environment,
        };

    #region IHostBuilder implementation

    public IDictionary<object, object> Properties => properties ??= new();
    protected Dictionary<object, object>? properties;

    public IConfiguration? Configuration => WrappedHostApplicationBuilder?.Configuration;

    public IConfigurationBuilder? ConfigurationBuilder => WrappedHostApplicationBuilder?.Configuration;

    public IHost Build() => WrappedHostApplicationBuilder.Build();

    public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        configureDelegate(WrappedHostApplicationBuilderContext, WrappedHostApplicationBuilder.Configuration);
        return this;
    }

    public IHostBuilder ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null)
        where TContainerBuilder : notnull
    {
        WrappedHostApplicationBuilder.ConfigureContainer<TContainerBuilder>(factory, configure);
        return this;
    }

    public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
    {
        throw new NotImplementedException();
        //ConfigureContainer(new ConfigureContainerAdapter<TContainerBuilder>(configureDelegate).Action);
        //return this;
    }

    #region From Microsoft

    //internal sealed class ConfigureContainerAdapter<TContainerBuilder> 
    //{
    //    internal Action<HostBuilderContext, TContainerBuilder> Action;

    //    public ConfigureContainerAdapter(Action<HostBuilderContext, TContainerBuilder> action)
    //    {
    //        Action = action ?? throw new ArgumentNullException();
    //    }

    //    //public void ConfigureContainer(HostBuilderContext hostContext, object containerBuilder)
    //    //{
    //    //    _action(hostContext, (TContainerBuilder)containerBuilder);
    //    //}
    //}
    #endregion

    public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
    {
        configureDelegate(WrappedHostApplicationBuilder.Configuration);
        return this;
    }

    public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        configureDelegate(WrappedHostApplicationBuilderContext, WrappedHostApplicationBuilder.Services); // DANGER: will throw at runtime if caller needs HostBuilderContext.
        return this;
    }

    public IHostBuilder ConfigureServices(Action<IServiceCollection> configureDelegate)
    {
        configureDelegate(WrappedHostApplicationBuilder.Services);
        return this;
    }

    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
        where TContainerBuilder : notnull
    {
        WrappedHostApplicationBuilder.ConfigureContainer(factory);
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
        where TContainerBuilder : notnull
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
    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
        where TContainerBuilder : notnull
        => throw new NotSupportedException();

    #endregion
}
