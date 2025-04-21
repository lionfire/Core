using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class ContainerizationHostingX
{
    public static bool IsInDockerContainer
        => File.Exists("/.dockerenv")
        || File.Exists("/proc/self/cgroup") && File.ReadAllText("/proc/self/cgroup").Contains("docker");


    public static string DefaultDockerConfigDir { get; set; } = "/app/conf"; // REVIEW: change to conf.d?

    public static IHostApplicationBuilder TryAddConfigForDocker(this IHostApplicationBuilder hostBuilder, bool reloadOnChange = true, bool force = false, bool optional = true, string? configDir = null)
    {
        if (!force && !IsInDockerContainer) return hostBuilder;

        var configuredConfigDir = hostBuilder.Configuration.GetValue<string>("ConfigDir");

        if (configuredConfigDir == null)
        {
            hostBuilder.AddConfigDir(configDir ?? DefaultDockerConfigDir, reloadOnChange: reloadOnChange, optional: optional);
        }

        return hostBuilder;
    }
}
