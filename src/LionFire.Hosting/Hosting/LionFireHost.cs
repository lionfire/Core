using Microsoft.Extensions.Hosting;
using LionFire.Hosting;
using System.ComponentModel.Design;
using Microsoft.Extensions.Configuration;

namespace LionFire.Hosting
{
    public static class LionFireHost
    {
        public static IHostBuilder Create(IConfiguration config = null, string[] args = null, bool defaultBuilder = true)
        {
            if(config == null && defaultBuilder)
            {
                config = HostConfiguration.GetConfiguration();
            }

            return (defaultBuilder ? Host.CreateDefaultBuilder(args) : new HostBuilder())
                .If(defaultBuilder, b => b.AddLionFireLogging(config))
                //.ConfigureContainer<LionFireDefaultServiceProviderFactory>(f => { })
                .UseServiceProviderFactory(new LionFireDefaultServiceProviderFactory())
                .ConfigureServices(services =>
                {
                    //services.AddSingleton<IServiceProviderFactory<>>
                });
        }
    }
}
