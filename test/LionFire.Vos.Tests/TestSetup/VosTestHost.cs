using Microsoft.Extensions.Hosting;
using LionFire.Hosting;

namespace LionFire.Services
{
    public static class VosTestHost
    {
        public static IHostBuilder Create(string[] args = null, bool defaultBuilder = true)
        {
            return VosHost.Create(args, defaultBuilder)
                .ConfigureServices(services 
                => services
                        .AddTestFileMount()
                        .AddFilesystem()
                        .VobEnvironment("test", "test")
                );
        }
    }
}
