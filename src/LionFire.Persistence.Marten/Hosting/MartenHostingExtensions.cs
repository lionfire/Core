using LionFire.Data.Marten.Connections;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Services;

namespace LionFire.Hosting
{
    public static class MartenHostingExtensions
    {
        public static IServiceCollection AddMartenConnections(this IServiceCollection services)
        {
            return services
                .AddSingletonHostedService<MartenConnectionManager>()
                ;
        }
    }
}
