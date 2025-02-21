using LionFire.Persistence.Filesystemlike;
using LionFire.Reactive.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class ReactivePersistenceHostingX
{
    public static IServiceCollection AddReactivePersistence(this IServiceCollection services)
    {
        return services
            .AddSingleton<ObservableCacheProvider>()
            //.AddFileExtensionConventions()
            ;
    }

    public static IServiceCollection AddFileExtensionConventions(this IServiceCollection services)
    {
        return services
            .AddSingleton<IFileExtensionConvention, DefaultExtensionConvention>()
            ;
    }
}
