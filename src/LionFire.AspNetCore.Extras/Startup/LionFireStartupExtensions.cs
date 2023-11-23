using Microsoft.Extensions.DependencyInjection;

namespace LionFire.AspNetCore;

public static class LionFireStartupExtensions
{
    /// <summary>
    /// Add:
    ///  - HealthChecksUI
    /// </summary>
    /// <param name="services"></param>
    /// <param name="startup"></param>
    /// <returns></returns>
    public static IServiceCollection AddLionFireServices(this IServiceCollection services/*, LionFireStartup startup*/)
    {
        services
            .AddLionFireHealthChecksUI();

        return services;
    }

    public static IServiceCollection AddLionFireHealthChecksUI(this IServiceCollection services/*, LionFireStartup startup*/)
    {
        services
             .AddHealthChecksUI(setupSettings: setup =>
             {
                 //setup.AddHealthCheckEndpoint("self", $"http://localhost:{startup.DefaultPort}/health/detail");
                 setup.AddHealthCheckEndpoint("self", "/health/detail");
             })
             //.AddSqliteStorage("Data Source=HealthCheckHistory.sqlite3")
             .AddInMemoryStorage()
             ;

        return services;
    }
}
