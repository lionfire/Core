using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;

namespace LionFire.AspNetCore;

public static class LionFireEndpointExtensions
{
    public static IEndpointRouteBuilder AddLionFire(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .AddLionFireHealthChecks()
            .AddLionFireHealthChecksUI()
            ;
    }

    public static IEndpointRouteBuilder AddLionFireHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health");
        endpoints.MapHealthChecks("/health/detail", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        return endpoints;
    }
    public static IEndpointRouteBuilder AddLionFireHealthChecksUI(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecksUI(config => config.UIPath = "/health-ui");
        return endpoints;
    }

}