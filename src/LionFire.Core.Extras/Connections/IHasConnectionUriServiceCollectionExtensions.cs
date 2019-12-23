using Microsoft.Extensions.DependencyInjection;
using System;

namespace LionFire.Services
{
    using LionFire.Data;

    public static class IHasConnectionUriServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureConnection<THasConnectionUri>(this IServiceCollection services, Uri connectionUri)
            where THasConnectionUri : class, IHasConnectionUri
        {
            services.Configure<THasConnectionUri>(o => o.ConnectionUri = connectionUri);
            return services;
        }

        }
}
