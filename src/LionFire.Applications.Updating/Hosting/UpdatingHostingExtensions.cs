using Microsoft.Extensions.DependencyInjection;
using System;

namespace LionFire.Applications.Updating
{
    public static class UpdatingHostingExtensions
    {
        public static IServiceCollection AddUpdating<T>(this IServiceCollection services)
            where T : IUpdateMechanism
        {
            return services;
        }
    }
}
