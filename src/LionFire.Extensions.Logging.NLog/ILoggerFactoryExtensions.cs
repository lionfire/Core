using System.Reflection;
using Microsoft.Extensions.Logging;
using NLog;
using LionFire.Extensions.Logging.NLog;

namespace LionFire.Extensions.Logging
{
    /// <summary>
    /// Helpers for ILoggerFactory
    /// </summary>
    public static class ILoggerFactoryExtensions
    {

        /// <summary>
        /// Enable NLog as logging provider in .NET Core.
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static ILoggerFactory AddNLog(this ILoggerFactory factory)
        {
            LogManager.AddHiddenAssembly(Assembly.Load(new AssemblyName("Microsoft.Extensions.Logging")));
            LogManager.AddHiddenAssembly(Assembly.Load(new AssemblyName("Microsoft.Extensions.Logging.Abstractions")));
            LogManager.AddHiddenAssembly(typeof(ILoggerFactoryExtensions).GetTypeInfo().Assembly);

            using (var provider = new NLogLoggerProvider())
            {
                factory.AddProvider(provider);
            }
            return factory;
        }
    }
}
