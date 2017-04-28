using LionFire.Structures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using MSLogger = Microsoft.Extensions.Logging.ILogger;

namespace LionFire.Extensions.Logging
{

    public static class LogInitializer
    {
        public static Func<string, MSLogger> GetLoggerMethod;

        private static MSLogger NullLogger => Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

        public static MSLogger GetLogger(this object obj, string name = null, bool isEnabled = true)
        {
            if (!isEnabled) return NullLogger;

            if (name == null) name = obj.GetType().FullName;

            if (GetLoggerMethod != null)
            {
                return GetLoggerMethod(name);
            }

            var fac = ManualSingleton<IServiceProvider>.Instance?.GetService<ILoggerFactory>();
            if (fac == null)
            {
                return NullLogger;
            }
            return fac.CreateLogger(name);
        }
    }
}
