#if FUTURE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Extensions.Logging.NLog
{
    public class ConfigExtensions
    {
        ///// <summary>
        ///// Apply NLog configuration from XML config.
        ///// </summary>
        ///// <param name="env"></param>
        ///// <param name="configFileRelativePath">relative path to NLog configuration file.</param>
        //public static void ConfigureNLog(this IHostingEnvironment env, string configFileRelativePath)
        //{
        //    var fileName = Path.Combine(env.ContentRootPath, configFileRelativePath);
        //    ConfigureNLog(fileName);
        //}

        ///// <summary>
        ///// Apply NLog configuration from XML config.
        ///// </summary>
        ///// <param name="fileName">absolute path  NLog configuration file.</param>
        //private static void ConfigureNLog(string fileName)
        //{
        //    LogManager.Configuration = new XmlLoggingConfiguration(fileName, true);
        //}
    }
}

#endif