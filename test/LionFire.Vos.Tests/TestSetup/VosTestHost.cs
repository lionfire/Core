using Microsoft.Extensions.Hosting;
using LionFire.Hosting;

namespace LionFire.Services
{
    public static class VosTestHost
    {
        public static IHostBuilder Create(string[] args = null, bool autoExtension = false)
        {
            return VosHost.Create(args)
                .ConfigureServices(services 
                => services
                        .AddTestFileMount(autoExtension)
                        .AddFilesystemPersister()
                        .AddNewtonsoftJson()
                        .VobEnvironment("test", "test")
                );
        }
    }
}
