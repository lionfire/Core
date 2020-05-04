using Microsoft.Extensions.Hosting;
using LionFire.Hosting;
using System.ComponentModel.Design;

namespace LionFire.Hosting
{
    public static class LionFireHost
    {
        public static IHostBuilder Create(string[] args = null, bool defaultBuilder = true)
            => (defaultBuilder ? Host.CreateDefaultBuilder(args) : new HostBuilder())
                //.ConfigureContainer<LionFireDefaultServiceProviderFactory>(f => { })
                .UseServiceProviderFactory(new LionFireDefaultServiceProviderFactory())
                .ConfigureServices(services =>
                {
                    //services.AddSingleton<IServiceProviderFactory<>>
                });
    }
}
