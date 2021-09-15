using LionFire.Hosting;
using LionFire.Infra.HealthChecker;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace LionFire.AspNetCore
{
    public static class LionFireWebHost
    {
        public static IHostBuilder Create<TStartup>(string[] args)
            where TStartup : class
        {
            LionFireHostingConfigurationUtils.TryCopyFromBaseDirectoryIfMissing("appsettings.json");
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults<TStartup>();
        }
    }
    
}
