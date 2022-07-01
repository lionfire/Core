using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Builder;

namespace LionFire.Hosting
{
    public static class WebApplicationBuilderExtensions
    {
        public static IHostBuilder LionFire(this WebApplicationBuilder webApplicationBuilder, Action<LionFireHostBuilder>? action = null, bool useDefaults = true) 
            => webApplicationBuilder.Host.LionFire(action, useDefaults);
    }
}
