using System;
using System.Collections.Generic;
using System.Text;

using LionFire.Structures;
using Microsoft.Extensions.Logging;
using LionFire.Logging.Null;

namespace LionFire
{
    // TODO: move this to LionFire.Logging?
    public static class Log
    {
        public static ILogger Get(string categoryName = null) => Defaults.TryGet<ILoggerFactory>(() => NullLoggerFactory.Instance)?.CreateLogger(categoryName);
        
    }
}
