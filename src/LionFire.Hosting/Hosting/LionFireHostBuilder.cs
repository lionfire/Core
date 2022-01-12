using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace LionFire.Hosting
{
    /// <summary>
    /// Marker type for fluent building of LionFire
    /// </summary>
    public class LionFireHostBuilder
    {
        public LionFireHostBuilder(IHostBuilder hostBuilder) { HostBuilder = new HostBuilderWrapper(hostBuilder, this); }

        public HostBuilderWrapper HostBuilder { get; }

        public LionFireHostBuilder ForHostBuilder(Action<IHostBuilder> action)
        {
            action(HostBuilder.Wrapped);
            return this;
        }

    }

    public class HostBuilderWrapper : IHostBuilder
    {
        public HostBuilderWrapper(IHostBuilder hostBuilder, LionFireHostBuilder parent)
        {
            Wrapped = hostBuilder; Parent = parent;
        }
        public IHostBuilder Wrapped { get; set; }
        public LionFireHostBuilder Parent
        { get; }

        public LionFireHostBuilder Done() => Parent;

        public IDictionary<object, object> Properties => Wrapped.Properties;

        public IHost Build()
        {
            return Wrapped.Build();
        }

        public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate) => Wrapped.ConfigureAppConfiguration(configureDelegate);

        public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate) => Wrapped.ConfigureContainer(configureDelegate);

        public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate) => Wrapped.ConfigureHostConfiguration(configureDelegate);

        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate) => Wrapped.ConfigureServices(configureDelegate);

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) => Wrapped.UseServiceProviderFactory(factory);

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) => Wrapped.UseServiceProviderFactory(factory);
    }

    public static class DoneExtensions
    {
        public static LionFireHostBuilder Done(this IHostBuilder hostBuilder) => (hostBuilder as HostBuilderWrapper)?.Done() 
            ?? throw new ArgumentException("hostBuilder is not HostBuilderWrapper and does not need Done")
            ;
    }
}
