
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LionFire.Cqrs;

#if Scrutor

public static class CqrsHostingX
{
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.TryAddSingleton<ICommandDispatcher, CommandDispatcher>();
        services.TryAddSingleton<IQueryDispatcher, QueryDispatcher>();
        services.AddCqrsQueryHandlers();
        services.AddCqrsCommandHandlers();
        return services;
    }
    public static IServiceCollection AddCqrsQueryHandlers(this IServiceCollection services)
    {
        // Using https://www.nuget.org/packages/Scrutor for registering all Query and Command handlers by convention
        services.Scan(selector =>
        {
            selector.FromCallingAssembly()
                    .AddClasses(filter =>
                    {
                        filter.AssignableTo(typeof(IQueryHandler<,>));
                    })
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime();
        });
        return services;
    }

    public static IServiceCollection AddCqrsCommandHandlers(this IServiceCollection services)
    {
        // Using https://www.nuget.org/packages/Scrutor for registering all Query and Command handlers by convention
        services.Scan(selector =>
        {
            selector.FromCallingAssembly()
                    .AddClasses(filter =>
                    {
                        filter.AssignableTo(typeof(ICommandHandler<,>));
                    })
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime();
        });
        return services;
    }
}

#endif