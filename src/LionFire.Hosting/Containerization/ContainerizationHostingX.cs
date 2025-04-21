using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

    public static IHostApplicationBuilder TryAddConfigForDocker(this IHostApplicationBuilder hostBuilder, bool reloadOnChange = true, bool force = false, string confDir = "/app/conf")
    {
        if (!force && !IsInDockerContainer) return hostBuilder;

        var configFile = Path.Combine(confDir, "appsettings.json");
        Log.Get(typeof(ReleaseChannelsHostingX).FullName).LogInformation($"Adding configuration source for docker: {configFile}");
        hostBuilder.Configuration.AddJsonFile(configFile, optional: true, reloadOnChange);
        return hostBuilder;
    }

}
