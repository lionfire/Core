using LionFire.Applications;
using LionFire.Dependencies;
using LionFire.DependencyMachines;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LionFire.Hosting;

public static class RunServicesExtensions
{
    public static IServiceCollection AddRunFiles(this IServiceCollection services)
        => services.AddParticipant(c =>
            {
                c.Key = "RunFileDirectory";
                c.Contributes("RunFiles");

                c.StartFunc = (sp, ct) =>
                    {
                        LionFireEnvironment.Directories.RunFileDirectory = Path.Combine(DependencyContext.Current.GetRequiredService<AppDirectories>().CompanyProgramData, "Run");
                        return null;
                    };

            });
}
