// MOVED to LionFire.Runtime.Extensions
//using LionFire.Structures;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace LionFire.Extensions.Logging
//{
//    public class NullLogger : ILogger
//    {
//        private class Disposable : IDisposable
//        {
//            public void Dispose()
//            {
//            }
//        }

//        public IDisposable BeginScope<TState>(TState state)
//        {
//            return Singleton<Disposable>.Instance;
//        }

//        public bool IsEnabled(LogLevel logLevel)
//        {
//            return false;
//        }

//        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
//        {
//        }
//    }
//}
