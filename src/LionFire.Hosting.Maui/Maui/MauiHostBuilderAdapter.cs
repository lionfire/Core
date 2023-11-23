using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting.Maui;

public class MauiHostBuilderAdapter : IHostBuilder
{
    private MauiAppBuilder builder;

    private HostBuilderContext HostBuilderContext = new(new Dictionary<object, object>());
    public IDictionary<object, object> Properties => HostBuilderContext.Properties;//  new Dictionary<object, object>();


    public MauiHostBuilderAdapter(MauiAppBuilder mauiAppBuilder)
    {
        builder = mauiAppBuilder;
    }

    public IHost Build() => new MauiHostAdapter(builder.Build()); // Alternate: do nothing here and do Build in MauiHostAdapter.StartAsync, since Building starts the app.

    public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        throw new NotImplementedException();
    }

    public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
    {
        throw new NotImplementedException();
        return this;
    }

    public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
    {
        throw new NotImplementedException();
    }

    public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        configureDelegate(HostBuilderContext, builder.Services);
        return this;
    }

    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
    {
        throw new NotImplementedException();
    }

    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
    {
        throw new NotImplementedException();
    }
}