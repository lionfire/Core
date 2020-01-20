using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Services;
using LionFire.Vos.VosApp;
using LionFire.Vos;

namespace LionFire.Hosting // REVIEW - consider changing this to LionFire.Services to make it easier to remember how to create a new app
{
    public static class VosAppHost
    {
        public static IHostBuilder Create(string[] args = null, VosAppOptions options = null, bool defaultBuilder = true, Action<IServiceCollection> serializers = null)
        {
            return VosHost.Create(args, defaultBuilder: defaultBuilder)
                     .ConfigureServices(services =>
                     {
                         services
                             .VobEnvironment("AppRoot", "/app".ToVosReference())
                             .VobEnvironment("internal", "/_".ToVosReference())
                             .VobEnvironment("stores", "/_/stores".ToVosReference()) // TODO: "$internal/stores"
                             .AddVosStores(b => b.Add)
                             .VobAlias("/`", "/app") // TODO, TOTEST
                             //.AddVosAppDefaultMounts(options) // TODO
                             //.VobAlias("/`", "$AppRoot") // FUTURE?  Once environment variables are ready
                             ;
                     })
                ;
        }
    }
}
