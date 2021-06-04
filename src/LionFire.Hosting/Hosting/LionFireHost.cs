using Microsoft.Extensions.Hosting;
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
                config = HostConfiguration.CreateDefault();
            }

            return (defaultBuilder ? Host.CreateDefaultBuilder(args) : new HostBuilder())
                .If(defaultBuilder, b => 
                b
                    .AddLionFireLogging(config)
                    .SetAsDefaultServiceProvider()
                )
                
                //.ConfigureContainer<LionFireDefaultServiceProviderFactory>(f => { })
                .UseServiceProviderFactory(new LionFireDefaultServiceProviderFactory())
                .ConfigureServices(services =>
                {
                    //services.AddSingleton<IServiceProviderFactory<>>
                });
        }
    }
}
