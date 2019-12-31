using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Services;

namespace LionFire.Hosting
{
    public static class VosAppHost
    {
        public static IHostBuilder Create(string[] args = null, bool defaultBuilder = true, Action<IServiceCollection> serializers = null)
        {
            return VosHost.Create(args, defaultBuilder: defaultBuilder)
                     .ConfigureServices(services =>
                     {
                         services
                             .VobEnvironment("AppRoot", "/app")
                             .VobAlias("/`", "/app")
                             //.VobAlias("/`", "/$AppRoot") // FUTURE?
                             ;
                     })
                ;
        }
    }
}
