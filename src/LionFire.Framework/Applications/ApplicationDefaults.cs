using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Applications
{
    public static class ApplicationDefaults
    {
        public static IServiceCollection AddApplicationDefaults(this IServiceCollection services)
        {
            return services
                //.AddSingleton<>
                ;
        }
    }
}
