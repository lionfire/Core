﻿using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace LionFire.Hosting;

public class LionFireWebApplicationBuilder : ILionFireHostBuilder
{
    public WebApplicationBuilder WebApplicationBuilder { get; }

    public LionFireWebApplicationBuilder(WebApplicationBuilder hostBuilder)
    {
        WebApplicationBuilder = hostBuilder;
    }

    public HostBuilderWrapper HostBuilder => throw new NotImplementedException(); // TODO - eliminate need for this

    public HostBuilderContext GetHostBuilderContext => new HostBuilderContext(WebApplicationBuilder.Host.Properties)
    {
        Configuration = WebApplicationBuilder.Configuration,
        HostingEnvironment = WebApplicationBuilder.Environment,
    };

    public ILionFireHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configure)
    {
        configure(GetHostBuilderContext, WebApplicationBuilder.Services);
        return this;
    }

    public ILionFireHostBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        configure(WebApplicationBuilder.Services);
        return this;
    }

    public ILionFireHostBuilder ForHostBuilder(Action<IHostBuilder> action)
    {
        throw new NotImplementedException();
    }

    public IConfiguration GetBootstrapConfiguration(LionFireHostBuilderBootstrapOptions? options = null)
        => WebApplicationBuilder.Configuration;
}