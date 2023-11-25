using System;
using LionFire.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Services;
using LionFire.FlexObjects;
using Microsoft.Extensions.Configuration;

namespace LionFire.Hosting;

public static class VosHost
{
    
    [Obsolete("Use LionFireHostBuilder")]
    public static IHostBuilder Create(string[] args = null) 
    {
        return PersistersHost.Create(args)
            .AddVos();
    }

    [Obsolete("Use LionFireHostBuilder")]
    public static HostApplicationBuilder Create_New(string[] args = null)
    {
        return PersistersHost.Create_New(args)
            .AddVos();
    }
}
