using LionFire.Structures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;


namespace LionFire.Extensions.Logging
{

    public static class LogInitializer
    {
        public static ILogger GetLogger(this object obj, string name = null, bool isEnabled = true)
        {
            if (name == null) name = obj.GetType().FullName;

            ILogger l;

            var fac = ManualSingleton<IServiceProvider>.Instance?.GetService<ILoggerFactory>();
            if (isEnabled && fac != null)
            {
                l = fac.CreateLogger(name);
            }
            else
            {
                l = Singleton<NullLogger>.Instance;
            }

            return l;
        }
    }
}
