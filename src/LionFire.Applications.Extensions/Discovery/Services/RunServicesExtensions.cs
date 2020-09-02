using LionFire.Applications;
using LionFire.DependencyMachines;
using LionFire.Services.DependencyMachines;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LionFire.Services
{
    public static class RunServicesExtensions
    {
        public static IServiceCollection AddRunFiles(this IServiceCollection services)
            => services.AddParticipant(c =>
                {
                    c.Key = "RunFileDirectory";
                    c.Contributes("RunFiles");

                    c.StartFunc = (sp, ct) =>
                        {
                            var appInfo = sp.ServiceProvider.GetRequiredService<AppInfo>();
                            LionFireEnvironment.Directories.RunFileDirectory = Path.Combine(appInfo.Directories.CompanyProgramData, "Run");
                            return null;
                        };

                });
    }
}
