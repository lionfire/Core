using LionFire.Structures.Keys;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class OrleansServiceCollectionExtensions
{
    public static IServiceCollection AddOrleans(this IServiceCollection services)
        => services
            .AddSingleton<IKeyProvider<string>, GrainKeyProvider>()
            ;
}
