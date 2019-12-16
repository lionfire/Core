using System;
using LionFire.Persistence;
using LionFire.Vos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Dependencies;

namespace LionFire.Hosting
{
    public static class VosHost
    {

        public static IHostBuilder Create(string[] args = null, bool defaultBuilder = true, Action<IServiceCollection> serializers = null)
        {
            return PersistersHost.Create(args, defaultBuilder: defaultBuilder)
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton<RootVob>();
                });
        }
    }
}
