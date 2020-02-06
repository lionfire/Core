using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using LionFire.Structures;
using Microsoft.Extensions.Logging;
using LionFire.Logging.Null;
using System.Runtime.CompilerServices;
using LionFire.Dependencies;

namespace LionFire
{
    // TODO: move this to LionFire.Logging?
    public static class Log
    {
        public static ILogger Get([CallerMemberName] string categoryName = null) => ServiceLocator.TryGet<ILoggerFactory>(() => NullLoggerFactory.Instance, tryCreateIfMissing: true)?.CreateLogger(categoryName);
        
    }
}
