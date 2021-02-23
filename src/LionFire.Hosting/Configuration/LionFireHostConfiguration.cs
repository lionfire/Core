#nullable enable
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace LionFire.Hosting
{
    public static class HostConfiguration
    {
        public static IConfigurationRoot CreateDefault(string? basePath = null)
            => CreateDefaultBuilder(basePath).Build();

        /// <summary>
        /// Get a typical configuration from appsettings.json and environment variables
        /// </summary>
        /// <returns></returns>
        public static IConfigurationBuilder CreateDefaultBuilder(string? basePath = null)
            => new ConfigurationBuilder()
               .SetBasePath(basePath ?? Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? System.IO.Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddEnvironmentVariables();
        
    }
}
