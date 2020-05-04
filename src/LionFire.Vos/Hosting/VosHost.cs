using System;
using LionFire.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Services;
using LionFire.FlexObjects;

namespace LionFire.Hosting
{
    public static class VosHost
    {
        public static IHostBuilder Create(string[] args = null, bool defaultBuilder = true, IFlex options = null) // Get rid of UNUSED options parameter?
        {
            return PersistersHost.Create(args, defaultBuilder: defaultBuilder)
                .AddVos();
        }
    }
}
