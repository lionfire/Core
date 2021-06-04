using LionFire.Data;
using LionFire.Data.LiteDB.Connections;
using LionFire.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting
{
    public static class LiteFsHostingExtensions
    {
        //public static IServiceCollection AddLiteFs(this IServiceCollection services)
        //{
        //    return services
        //        .AddConnectionManager<LiteDbConnection, LiteDbConnectionOptions, LiteDbConnectionManager>()
        //        .AddHostedService(sp => sp.GetRequiredService<LiteDbConnectionManager>())
        //        ;
        //}

        public static IHostBuilder AddLiteFs(this IHostBuilder builder)
        {
            return builder
                .AddConnectionManager<LiteDbConnection, LiteDbConnectionOptions, LiteDbConnectionManager>()
                .ConfigureServices(s=> s.AddHostedService(sp => sp.GetRequiredService<LiteDbConnectionManager>()))
                ;
        }
    }
}
