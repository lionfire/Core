using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using LionFire.Structures;
using TLogger = Microsoft.Extensions.Logging.ILogger;

namespace LionFire.Logging.Null
{
    public class NullLoggerFactory : ILoggerFactory
    {
        public static NullLoggerFactory Instance => Singleton<NullLoggerFactory>.Instance;

        public void AddProvider(ILoggerProvider provider) { }

        public TLogger CreateLogger(string categoryName) => Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

        public void Dispose() { }


    }
}
