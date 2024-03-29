﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace LionFire.Hosting
{
    public static class RadzenHostingExtensions
    {
        public static ILionFireHostBuilder Radzen(this ILionFireHostBuilder b)
            => b.ForHostBuilder(b=>b.ConfigureServices(services => services.AddRadzen()));

        public static IServiceCollection AddRadzen(this IServiceCollection services)
            => services
                    .AddScoped<DialogService>()
                    .AddScoped<NotificationService>()
                    .AddScoped<TooltipService>()
                    .AddScoped<ContextMenuService>();

    }
}
