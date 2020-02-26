using LionFire.Persistence.AutoExtensionFilesystem;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Services
{
    public static class AutoExtensionFilesystemServicesExtensions
    {
        public static IServiceCollection AddAutoExtensionFilesystem(this IServiceCollection services)
        {
            return services
                .AddSingleton<AutoExtensionFilesystemPersister>()
                .AddSingleton(serviceProvider => serviceProvider.GetService<IOptionsMonitor<AutoExtensionFilesystemPersisterOptions>>().CurrentValue)
                ;
        }
    }
}
