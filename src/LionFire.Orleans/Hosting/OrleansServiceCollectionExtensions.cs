using LionFire.Structures;
using LionFire.Structures.Keys;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class OrleansStaticInitialization
{
    public static void InitOrleans()
    {
        StaticKeySelector<IAddressable, GrainId>.Selector = a => a.GetGrainId();
        StaticKeySelector<IAddressable, string>.Selector = a => a.GetGrainId().Key.ToString();
    }
}

public static class OrleansServiceCollectionExtensions
{
    public static IServiceCollection AddOrleans(this IServiceCollection services)
        => services
            .AddSingleton<IKeyProviderStrategy<string>, GrainKeyProvider>()
            ;

    public static IServiceCollection AddOrleansStatics(this IServiceCollection services)
    {
        OrleansStaticInitialization.InitOrleans();        
        return services;
    }

    //public static IHostBuilder AddOrleansClientX(this IHostBuilder services)
    //    => services
    //        .UseOrleansClient(c=>c)
    //        ;
}
