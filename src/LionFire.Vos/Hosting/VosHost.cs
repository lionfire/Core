using System;
using LionFire.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Services;
using LionFire.FlexObjects;
using Microsoft.Extensions.Configuration;

namespace LionFire.Hosting
{
    public static class VosHost
    {
        //public static IHostBuilder CreateVosHost(this IHostBuilder hostBuilder, IConfiguration config = null, string[] args = null, bool defaultBuilder = true, IFlex options = null)
        //    => Create(config, args, defaultBuilder, hostBuilder, options);

        [Obsolete]
        public static IHostBuilder Create(string[] args = null) 
            // Get rid of UNUSED options parameter?
        {
            return PersistersHost.Create(args)
                .AddVos();
        }
    }
}
