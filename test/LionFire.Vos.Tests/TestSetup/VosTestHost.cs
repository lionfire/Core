using Microsoft.Extensions.Hosting;
using LionFire.Hosting;

namespace LionFire.Services
{
    public static class VosTestHost
    {
        public static IHostBuilder Create(string[] args = null, bool defaultBuilder = true, bool autoExtension = false)
        {
            return VosHost.Create(args, defaultBuilder)
                .ConfigureServices(services 
                => services
                        .AddTestFileMount(autoExtension)
                        .AddFilesystem()
                        .AddNewtonsoftJson()
                        .VobEnvironment("test", "test")
                );
        }
    }
}
