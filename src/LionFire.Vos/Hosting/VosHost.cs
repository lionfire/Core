using System;
using LionFire.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Services;

namespace LionFire.Hosting
{
    public static class VosHost
    {
        public static IHostBuilder Create(string[] args = null, bool defaultBuilder = true)
        {
            return PersistersHost.Create(args, defaultBuilder: defaultBuilder)
                .AddVos();
        }
    }
}
