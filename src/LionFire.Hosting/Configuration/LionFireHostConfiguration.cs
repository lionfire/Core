using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace LionFire.Hosting
{
    // REVIEW - better name for this class and GetConfiguration method?

    public static class HostConfiguration
    {
        /// <summary>
        /// Get a typical configuration from appsettings.json and environment variables
        /// </summary>
        /// <returns></returns>
        public static IConfigurationRoot GetConfiguration() 
            => new ConfigurationBuilder()
               .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? System.IO.Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();
    }
}
