using LionFire.Data.Async.Gets.ChainResolving;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Services
{
    public static class ChainResolverServicesExtensions
    {
        public static IServiceCollection AddChainResolver<TFrom, TTo>(this IServiceCollection services, Func<TFrom, TTo> converter)
            => services.Configure<ChainResolveOptions>(options => options.Resolvers.Add(new ChainResolverWorker(typeof(TTo), obj => converter((TFrom)obj))));
    }
}
