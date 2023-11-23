using Microsoft.Extensions.Hosting;
using LionFire.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting
{
    public static class SetterOfDefaultServiceProviderExtensions
    {
        public static IServiceCollection SetAsDefaultServiceProvider(this IServiceCollection services)
            => services.AddHostedService<SetterOfDefaultServiceProvider>();
        public static IHostBuilder SetAsDefaultServiceProvider(this IHostBuilder hostBuilder)
            => hostBuilder.ConfigureServices(s => s.AddHostedService<SetterOfDefaultServiceProvider>());
    }
}
