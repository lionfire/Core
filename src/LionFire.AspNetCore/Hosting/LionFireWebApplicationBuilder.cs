using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace LionFire.Hosting;

public class LionFireWebApplicationBuilder : ILionFireHostBuilder
{
    public WebApplicationBuilder HostBuilder1 { get; }

    public LionFireWebApplicationBuilder(WebApplicationBuilder hostBuilder)
    {
        HostBuilder1 = hostBuilder;
    }

    public HostBuilderWrapper HostBuilder => throw new NotImplementedException(); // TODO - eliminate need for this

    public HostBuilderContext GetHostBuilderContext => new HostBuilderContext(HostBuilder1.Host.Properties)
    {
        Configuration = HostBuilder1.Configuration,
        HostingEnvironment = HostBuilder1.Environment,
    };

    public ILionFireHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configure)
    {
        configure(GetHostBuilderContext, HostBuilder1.Services);
        return this;
    }

    public ILionFireHostBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        configure(HostBuilder1.Services);
        return this;
    }

    public ILionFireHostBuilder ForHostBuilder(Action<IHostBuilder> action)
    {
        throw new NotImplementedException();
    }

    public IConfiguration GetBootstrapConfiguration(LionFireHostBuilderBootstrapOptions options)
        => HostBuilder1.Configuration;
}
