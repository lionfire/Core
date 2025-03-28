﻿using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace LionFire.Hosting;

// REVIEW - is this obsolete with .NET 8 and IHostApplicationBuilder?
public class LionFireWebApplicationBuilder : HostApplicationSubBuilder, ILionFireHostBuilder
{

    public WebApplicationBuilder WebApplicationBuilder { get; }
    public override IHostApplicationBuilder IHostApplicationBuilder => WebApplicationBuilder;

    private HostApplicationBuilder HostApplicationBuilder { get; }

    #region IHostBuilder

    // TODO - eliminate need for this?
    public IHostBuilder IHostBuilder { get; }
    //public LionFireHostBuilderWrapper HostBuilder => hostApplicationBuilder throw new NotImplementedException(); 

    #endregion

    public LionFireHostBuilderWrapper HostBuilder { get; }

    #region Lifecycle

    public LionFireWebApplicationBuilder(WebApplicationBuilder hostBuilder)
    {
        WebApplicationBuilder = hostBuilder;
        HostApplicationBuilder = hostBuilder.ToHostApplicationBuilder();
        IHostBuilder = new HostApplicationBuilderToIHostBuilderAdapter(HostApplicationBuilder);
        HostBuilder = new LionFireHostBuilderWrapper(HostApplicationBuilder, this);
    }

    #endregion

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
    public ILionFireHostBuilder ForHostApplicationBuilder(Action<HostApplicationBuilder> action)
    {
        throw new NotImplementedException();
    }


    public IConfiguration GetConfigurationForLogBootstrap(LionFireHostBuilderBootstrapOptions? options = null)
        => WebApplicationBuilder.Configuration;

    public ILionFireHostBuilder ForIHostApplicationBuilder(Action<IHostApplicationBuilder> action)
    {
        action(WebApplicationBuilder);
        return this;
    }

    public ILionFireHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> action)
    {
        throw new NotImplementedException();
    }

    public ILionFireHostBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> action)
    {
        throw new NotImplementedException();
    }
}
