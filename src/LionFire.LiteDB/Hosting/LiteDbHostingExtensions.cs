using LionFire.Data.LiteDB.Connections;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Services;

namespace LionFire.Hosting
{
    public static class LiteDbHostingExtensions
    {
        public static IServiceCollection AddLiteDbConnections(this IServiceCollection services)
        {
            return services
                .AddSingletonHostedService<LiteDbConnectionManager>()
                ;
        }
    }
}
