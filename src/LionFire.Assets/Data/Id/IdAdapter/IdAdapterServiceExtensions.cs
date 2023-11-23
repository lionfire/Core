using LionFire.Data.Id;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

public static class IdAdapterServiceExtensions
{
    public static IServiceCollection AddIdAdapter(this IServiceCollection services)
        => services
            .AddSingleton<IdAdapter>()
            ;
    public static IServiceCollection AddIdAdapterDefaultStrategies(this IServiceCollection services)
            => services
                .Configure<IdAdapterConfiguration>(c =>
                c.Strategies
                    .AddRange(new IIdMappingStrategy[] {
                                new StringIdedIdAdapterStrategy(),
                                new KeyedIdAdapterStrategy(),
                                //new NamedIdAdapterStrategy(), // OLD - dupe of KeyedIdAdapterStrategy
                                new AttributeOnPropertyIdAdapterStrategy(),
                    })
                );
}
