using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;

namespace LionFire.Hosting;

public static class ConfigDirHostingX
{
    public static IHostApplicationBuilder AddConfigDir(this IHostApplicationBuilder hostBuilder, string? configDir, bool reloadOnChange = true, bool optional = true)
    {
        if (configDir != null)
        {
            var configFile = Path.Combine(configDir, "appsettings.json");
            Log.Get(typeof(ConfigDirHostingX).FullName!).LogInformation($"Adding configuration file: {configFile}");
            hostBuilder.Configuration.AddJsonFile(configFile, optional: optional, reloadOnChange);
        }
        return hostBuilder;
    }

}
