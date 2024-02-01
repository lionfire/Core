using LionFire.Context.Parsing;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Context;

public static class ContextHostingX
{
    public static IServiceCollection AddContext(this IServiceCollection services)
        => services.AddSingleton<IContextParser, ContextParser>();
}
