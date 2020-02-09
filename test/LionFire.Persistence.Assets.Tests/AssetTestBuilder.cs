using LionFire.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using LionFire.Services;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Assets.Tests
{
    public static class AssetTestBuilder
    {
        public static IHostBuilder Create()
        {
            return VosHost.Create()
                .ConfigureServices((context, services) =>
                {

                    services
                        .VobEnvironment<string>("assets", "testAssets")
                        //.AddAssetPersister() // default path: $assets ?? /$internal/assets
                                           //.AddSingleton<string>()
                        ;
                })
                ;
        }
    }
}
