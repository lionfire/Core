using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace LionFire.Hosting;

// TODO: With .NET 8, standardize on IHostApplicationBuilder (WrappedIHostApplicationBuilder) as much as possible.  The nice thing is WebApplicationBuilder now inherits from IHostApplicationBuilder.
// TODO: Refactor this with LionFireHostBuilder, consider support for other builder types: ASP.NET Core, MAUI
public class LionFireHostBuilderWrapper : IHostBuilder
{

    public LionFireHostBuilderWrapper(IHostBuilder hostBuilder, ILionFireHostBuilder? parent = null)
    {
        WrappedHostBuilder = hostBuilder;
        Parent = parent;
    }

    public LionFireHostBuilderWrapper(IHostApplicationBuilder hostApplicationBuilder, ILionFireHostBuilder? parent = null)
    {
        WrappedIHostApplicationBuilder = hostApplicationBuilder;
        //WrappedHostBuilder = new HostBuilderAdapter(WrappedHostApplicationBuilder); // Exists internally to Microsoft code
        Parent = parent;
    }

    public IHostBuilder? WrappedHostBuilder { get; set; }

#if OLD
    //[Obsolete("Use WrappedIHostApplicationBuilder instead")]
    //public HostApplicationBuilder? WrappedHostApplicationBuilder => WrappedIHostApplicationBuilder as HostApplicationBuilder;
#endif
    public IHostApplicationBuilder WrappedIHostApplicationBuilder { get; set; }

    public ILionFireHostBuilder? Parent { get; }

#if UNUSED
    [Obsolete("Use ForHostBuilder")]
    public ILionFireHostBuilder Done() => Parent;
#endif

    Exception NotSupportedForHostBuilderException => new NotSupportedException($"Not supported for {nameof(IHostBuilder)}");
    public IConfiguration Configuration => WrappedIHostApplicationBuilder?.Configuration 
        ?? throw NotSupportedForHostBuilderException; // TODO: How to get Configuration? Build first?
    //(WrappedHostBuilder.Build().Services.GetService<IConfiguration>());

    public IConfigurationBuilder ConfigurationBuilder => WrappedIHostApplicationBuilder?.Configuration ?? throw NotSupportedForHostBuilderException; // TODO: How to get IConfigurationBuilder?

    public IDictionary<object, object> Properties =>
        WrappedIHostApplicationBuilder != null ? WrappedIHostApplicationBuilder.Properties
        : (WrappedHostBuilder != null ? WrappedHostBuilder.Properties : (properties ??= new()));
    public Dictionary<object, object>? properties = null;

    public IHost Build()
    {
        if (WrappedHostBuilder != null) return WrappedHostBuilder.Build();
#if OLD
//#pragma warning disable CS0618 // Type or member is obsolete
//        if (WrappedHostApplicationBuilder != null) return WrappedHostApplicationBuilder.Build();
//#pragma warning restore CS0618 // Type or member is obsolete
#endif
        if (WrappedIHostApplicationBuilder is HostApplicationBuilder hab) return hab.Build();

        if (WrappedIHostApplicationBuilder != null)
        {
            var mi = WrappedIHostApplicationBuilder.GetType().GetMethod("Build", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (mi != null)
            {
                var result = mi.Invoke(WrappedIHostApplicationBuilder, null);
                if (result is IHost host) return host;
                else
                {
                    throw new Exception("Unknown builder type. Built but don't know how to cast to IHost.");
                }
            }
        }

        throw new NotSupportedException();
    }

    public HostBuilderContext WrappedHostApplicationBuilderContext
        => new(Properties)
        {
            Configuration = WrappedIHostApplicationBuilder.Configuration,
            HostingEnvironment = WrappedIHostApplicationBuilder.Environment,
        };

    public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        if (WrappedHostBuilder != null)
        {
            WrappedHostBuilder.ConfigureAppConfiguration(configureDelegate);
        }
        else if (WrappedIHostApplicationBuilder != null)
        {
            configureDelegate(WrappedHostApplicationBuilderContext, WrappedIHostApplicationBuilder.Configuration);
        }
        else throw NoHostBuilderException;

        return this;
    }

    // Not Needed?
    public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
    {
        if (WrappedHostBuilder != null)
        {
            WrappedHostBuilder.ConfigureContainer(configureDelegate);
        }
        else if (WrappedIHostApplicationBuilder != null)
        {
            throw new NotSupportedException(); // HostBuilderContext not available. Try the other overload.
            //WrappedIHostApplicationBuilder.ConfigureContainer<TContainerBuilder>(configureDelegate);
        }
        else throw NoHostBuilderException;

        return this;
    }
    /*
    public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<TContainerBuilder> configureDelegate)
        where TContainerBuilder : notnull
    {
        if (WrappedHostBuilder != null)
        {
            WrappedHostBuilder.ConfigureContainer(configureDelegate);
        }
        else if (WrappedIHostApplicationBuilder != null)
        {
            throw new NotSupportedException();
            //WrappedIHostApplicationBuilder.ConfigureContainer<TContainerBuilder>(configureDelegate);
        }
        else throw NoHostBuilderException;

        return this;
    }
    public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<TContainerBuilder> configureDelegate)
        where TContainerBuilder : notnull
    {
        if (WrappedHostBuilder != null)
        {
            throw new NotSupportedException();
        }
        else if (WrappedIHostApplicationBuilder != null)
        {
            //WrappedIHostApplicationBuilder.ConfigureContainer<TContainerBuilder>(configureDelegate);
        }
        else throw NoHostBuilderException;

        return this;
    }*/

    public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
    {
        if (WrappedHostBuilder != null)
        {
            WrappedHostBuilder.ConfigureHostConfiguration(configureDelegate);
        }
        else if (WrappedIHostApplicationBuilder != null)
        {
            configureDelegate(WrappedIHostApplicationBuilder.Configuration);
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
        else if (WrappedIHostApplicationBuilder != null)
        {
            configureDelegate(WrappedHostApplicationBuilderContext, WrappedIHostApplicationBuilder.Services); // DANGER: will throw at runtime if caller needs HostBuilderContext.
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
        else if (WrappedIHostApplicationBuilder != null)
        {
            configureDelegate(WrappedIHostApplicationBuilder.Services);
        }
        else throw NoHostBuilderException;

        return this;
    }

    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
        where TContainerBuilder : notnull
    {
        if (WrappedHostBuilder != null) WrappedHostBuilder.UseServiceProviderFactory(factory);
        else WrappedIHostApplicationBuilder.ConfigureContainer(factory);
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
        WrappedIHostApplicationBuilder.ConfigureContainer<TContainerBuilder>(factory, configure);
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
        => (WrappedHostBuilder ?? throw new NotSupportedException()).UseServiceProviderFactory(factory);

    #region Exceptions

    Exception NoHostBuilderException => new ArgumentNullException($"One of these must be set: {nameof(WrappedHostBuilder)}, {nameof(WrappedIHostApplicationBuilder)}");


    #endregion
}
